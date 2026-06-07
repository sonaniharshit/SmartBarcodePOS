/* ============================================================
   SMART BARCODE POS PRO - POS BILLING JAVASCRIPT
   Handles: barcode scanner, product lookup, cart management,
   discount calculation, and bill generation.
   ============================================================ */


/* ------------------------------------------------------------
   1. MODULE STATE
   ------------------------------------------------------------ */

let scanner        = null;   // Html5Qrcode instance
let currentProduct = null;   // Last product fetched from server
let cart           = [];     // Array of cart items

// Scanner duplicate protection
let isProcessingScan = false;
let lastScannedText  = '';
let lastScanTime     = 0;


/* ------------------------------------------------------------
   2. UTILITY: MONEY HELPERS
   ------------------------------------------------------------ */

/**
 * Safely rounds a value to 2 decimal places,
 * avoiding floating-point drift.
 */
function money(value) {
    const n = Number(value || 0);
    return Math.round((n + Number.EPSILON) * 100) / 100;
}

/**
 * Returns a money value formatted as a 2-decimal string.
 */
function formatMoney(value) {
    return money(value).toFixed(2);
}


/* ------------------------------------------------------------
   3. DOM READY - BIND EVENTS
   ------------------------------------------------------------ */

$(function () {

    // Scanner controls
    $('#btnStartScanner').click(startScanner);
    $('#btnStopScanner').click(stopScanner);

    // Product lookup
    $('#btnFindProduct').click(function () {
        fetchProduct($('#manualSku').val());
    });

    // Enter key in SKU field
    $('#manualSku').keypress(function (e) {
        if (e.which === 13) fetchProduct($('#manualSku').val());
    });

    // Cart actions
    $('#btnAddToCart').click(addToCart);

    $('#btnClearCart').click(function () {
        cart = [];
        $('#discountPercent').val(0);
        renderCart();
        toastr.info('Cart cleared');
    });

    // Bill generation
    $('#btnGenerateBill').click(generateBill);

    // Live discount recalculation
    $('#discountPercent').on('input change', renderCart);

});



/* ------------------------------------------------------------
   SCAN BEEP SOUND
   Uses Web Audio API, no audio file required.
   ------------------------------------------------------------ */
function playScanBeep() {
    try {
        const AudioContext = window.AudioContext || window.webkitAudioContext;
        if (!AudioContext) return;

        const ctx = new AudioContext();
        const oscillator = ctx.createOscillator();
        const gain = ctx.createGain();

        oscillator.type = 'sine';
        oscillator.frequency.value = 980;

        gain.gain.setValueAtTime(0.001, ctx.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.25, ctx.currentTime + 0.01);
        gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.12);

        oscillator.connect(gain);
        gain.connect(ctx.destination);

        oscillator.start(ctx.currentTime);
        oscillator.stop(ctx.currentTime + 0.13);
    } catch {
        // Ignore sound errors, scanning should continue.
    }
}


/* ------------------------------------------------------------
   4. BARCODE SCANNER
   ------------------------------------------------------------ */


function showScannerLoading(message = 'Opening camera...') {
    $('#reader')
        .addClass('scanner-loading')
        .removeClass('scanner-active scanner-success')
        .html(`
            <div class="scanner-state">
                <div class="scanner-spinner"></div>
                <h6 class="fw-bold mt-3 mb-1">${message}</h6>
                <small>Please allow camera permission</small>
            </div>
        `);
}

function showScannerActive() {
    $('#reader')
        .removeClass('scanner-loading scanner-success')
        .addClass('scanner-active');

    if (!$('#reader .scanner-line').length) {
        $('#reader').append('<div class="scanner-line"></div><div class="scanner-corners"></div>');
    }
}

function showScannerSuccess(scannedText) {
    $('#reader')
        .removeClass('scanner-loading scanner-active')
        .addClass('scanner-success')
        .html(`
            <div class="scanner-state">
                <i class="fa-solid fa-circle-check fa-3x text-success mb-2"></i>
                <h6 class="fw-bold mb-1">Barcode Scanned</h6>
                <small>${scannedText}</small>
            </div>
        `);
}

function startScanner() {

    if (scanner) {
        toastr.info('Scanner already running');
        return;
    }

    showScannerLoading('Opening camera...');

    scanner = new Html5Qrcode('reader');
    isProcessingScan = false;
    lastScannedText = '';
    lastScanTime = 0;

    scanner
        .start(
            { facingMode: 'environment' },
            { fps: 10, qrbox: { width: 260, height: 130 } },
            function (scannedText) {

                scannedText = (scannedText || '').trim();
                if (!scannedText) return;

                const now = Date.now();

                // Prevent same barcode firing repeatedly while camera is still pointed at it
                if (isProcessingScan || (scannedText === lastScannedText && now - lastScanTime < 1800)) {
                    return;
                }

                isProcessingScan = true;
                lastScannedText = scannedText;
                lastScanTime = now;

                $('#manualSku').val(scannedText);
                playScanBeep();

                // Mobile vibration
                if (navigator.vibrate) {
                    navigator.vibrate(100);
                }

                // Green success flash
                $('#reader').addClass('scan-success');

                setTimeout(() => {
                    $('#reader').removeClass('scan-success');
                }, 300);

                // Keep scanner running. Do NOT stop scanner after first scan.
                fetchProduct(scannedText, true);
            }
        )
        .then(function ()  {
            showScannerActive();
            toastr.info('Scanner started. You can scan multiple products.');
        })
        .catch(function () {
            scanner = null;
            isProcessingScan = false;
            resetReader();
            toastr.error('Camera permission required');
        });

}

function stopScanner(showToast = true) {

    isProcessingScan = false;
    lastScannedText = '';
    lastScanTime = 0;

    if (scanner) {
        scanner
            .stop()
            .then(function () {
                scanner.clear();
                scanner = null;
                resetReader();
                if (showToast) toastr.info('Scanner stopped');
            })
            .catch(function () {
                try {
                    scanner?.clear();
                } catch { }

                scanner = null;
                resetReader();

                if (showToast) toastr.info('Scanner stopped');
            });
    } else {
        resetReader();
    }

}

function resetReader() {
    $('#reader')
        .removeClass('scanner-loading scanner-active scanner-success')
        .html(`
            <div>
                <i class="fa-solid fa-camera fa-3x mb-3"></i>
                <h6>Camera Scanner</h6>
                <small>Click Open Camera to scan barcode</small>
            </div>
        `);
}


/* ------------------------------------------------------------
   5. PRODUCT LOOKUP
   ------------------------------------------------------------ */



function fetchProduct(sku, fromScanner = false) {

    sku = (sku || '').trim();

    if (!sku) {
        toastr.warning('Enter SKU');
        isProcessingScan = false;
        return;
    }

    $.get('/Billing/GetProductBySku', { sku })
        .done(function (res) {

            currentProduct       = res.data;
            currentProduct.price = money(currentProduct.price);

            // Populate product preview panel
            $('#previewName').text(currentProduct.productName);
            $('#previewSku').text(currentProduct.sku);
            $('#previewCategory').text(currentProduct.categoryName);
            $('#previewPrice').text(formatMoney(currentProduct.price));
            $('#previewStock').text(currentProduct.stockQty);

            $('#productPreview').removeClass('d-none');

            if (fromScanner) {
                // Real POS flow: camera scan directly adds to cart
                addProductDirectlyToCart(currentProduct);
                toastr.success('Scanned and added to cart');
            } else {
                // Manual SKU flow: show preview, user clicks Add Item
                toastr.success('Product found');
            }

        })
        .fail(function (xhr) {
            currentProduct = null;
            toastr.error(xhr.responseJSON?.message || 'Product not found');
        })
        .always(function () {
            setTimeout(function () {
                isProcessingScan = false;
            }, 700);
        });

}



function addProductDirectlyToCart(product) {

    if (!product) return;

    product.price = money(product.price);

    let item = cart.find(x => x.productId === product.productId);

    if (item) {
        if (item.qty + 1 > item.stockQty) {
            toastr.warning('Not enough stock for ' + item.productName);
            return;
        }

        item.qty++;
    } else {
        cart.push({
            ...product,
            qty: 1,
            price: money(product.price)
        });
    }

    renderCart();
}

/* ------------------------------------------------------------
   6. CART MANAGEMENT
   ------------------------------------------------------------ */

function addToCart() {

    if (!currentProduct) {
        toastr.warning('Find or scan product first');
        return;
    }

    addProductDirectlyToCart(currentProduct);
    toastr.success('Added to cart');

}

function changeQty(productId, delta) {

    let item = cart.find(x => x.productId === productId);
    if (!item) return;

    const newQty = item.qty + delta;

    if (newQty <= 0) {
        removeItem(productId);
        return;
    }

    if (newQty > item.stockQty) {
        toastr.warning('Not enough stock');
        return;
    }

    item.qty = newQty;
    renderCart();

}

function removeItem(productId) {
    cart = cart.filter(x => x.productId !== productId);
    renderCart();
}


/* ------------------------------------------------------------
   7. TOTALS CALCULATION
   ------------------------------------------------------------ */

function totals() {

    const sub = money(
        cart.reduce((sum, item) => sum + (money(item.price) * item.qty), 0)
    );

    let discountPercent = money($('#discountPercent').val());
    if (discountPercent < 0)   discountPercent = 0;
    if (discountPercent > 100) discountPercent = 100;

    const discountAmount = money((sub * discountPercent) / 100);
    const total          = money(sub - discountAmount);

    return {
        sub,
        discountPercent,
        discountAmount,
        total,
    };

}


/* ------------------------------------------------------------
   8. CART RENDER
   ------------------------------------------------------------ */

function renderCart() {

    const body = $('#cartBody');
    body.empty();

    if (cart.length === 0) {
        body.html('<tr><td colspan="5" class="text-center text-muted py-4">No item added</td></tr>');
    } else {
        cart.forEach(function (item) {
            body.append(`
                <tr class="cart-mobile-row">
                    <td data-label="Product">
                        <div class="item-name">${item.productName}</div>
                        <div class="item-code">${item.sku}</div>
                    </td>
                    <td data-label="Price">₹${formatMoney(item.price)}</td>
                    <td data-label="Qty">
                        <div class="qty-control">
                            <button type="button" onclick="changeQty(${item.productId}, -1)">-</button>
                            <span>${item.qty}</span>
                            <button type="button" onclick="changeQty(${item.productId}, 1)">+</button>
                        </div>
                    </td>
                    <td data-label="Total"><b>₹${formatMoney(item.price * item.qty)}</b></td>
                    <td data-label="Action">
                        <button type="button" class="remove-btn" onclick="removeItem(${item.productId})">
                            <i class="fa-solid fa-xmark"></i>
                        </button>
                    </td>
                </tr>
            `);
        });
    }

    // Update summary panel
    const t = totals();

    $('#discountPercent').val(t.discountPercent);
    $('#subTotal').text('₹' + formatMoney(t.sub));
    $('#discountAmountDisplay').text(
        '₹' + formatMoney(t.discountAmount) +
        ' (' + formatMoney(t.discountPercent) + '%)'
    );
    $('#grandTotal').text('₹' + formatMoney(t.total));
    $('#totalItems').text(cart.length);
    $('#totalQty').text(cart.reduce((sum, x) => sum + x.qty, 0));

}


/* ------------------------------------------------------------
   9. BILL GENERATION
   ------------------------------------------------------------ */

function generateBill() {

    if (cart.length === 0) {
        toastr.warning('Cart is empty');
        return;
    }

    const t = totals();

    if (t.discountPercent < 0 || t.discountPercent > 100) {
        toastr.error('Discount percent must be between 0 and 100.');
        return;
    }

    const payload = {
        items: cart.map(function (item) {
            return {
                productId:   item.productId,
                sku:         item.sku,
                productName: item.productName,
                price:       money(item.price),
                qty:         item.qty,
            };
        }),
        discountPercent: t.discountPercent,
        discountAmount:  t.discountAmount,
    };

    $.ajax({
        url:         '/Billing/GenerateBill',
        type:        'POST',
        contentType: 'application/json',
        data:        JSON.stringify(payload),
    })
        .done(function (res) {
            toastr.success(res.message + ' Bill: ' + res.billNo);

            // Open print page after short delay
            setTimeout(function () {
                window.open(res.printUrl, '_blank');
            }, 700);

            // Clear cart
            cart = [];
            $('#discountPercent').val(0);
            renderCart();
        })
        .fail(function (xhr) {
            toastr.error(xhr.responseJSON?.message || 'Bill failed');
        });

}
