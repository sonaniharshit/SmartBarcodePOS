/* ============================================================
   CASHIER PORTAL - MOBILE POS JAVASCRIPT
   Handles: tab switching, barcode scanner, product lookup,
   cart management, discount, bill generation.
   Endpoint base: /CashierPortal/
   ============================================================ */


/* ============================================================
   1. STATE
   ============================================================ */

let scanner          = null;
let currentProduct   = null;
let cart             = [];

let isProcessingScan = false;
let lastScannedText  = '';
let lastScanTime     = 0;

let activeTab        = 'scan';


/* ============================================================
   2. MONEY HELPERS
   ============================================================ */

function money(v) {
    return Math.round((Number(v || 0) + Number.EPSILON) * 100) / 100;
}

function fmt(v) {
    return money(v).toFixed(2);
}


/* ============================================================
   3. TAB SWITCHING
   ============================================================ */

function switchTab(tab) {
    activeTab = tab;

    // Pane visibility
    document.querySelectorAll('.cashier-tab-pane').forEach(p => p.classList.remove('active'));
    const pane = document.getElementById('tab-' + tab);
    if (pane) pane.classList.add('active');

    // Top tabs highlight
    document.querySelectorAll('.cashier-tab').forEach(t => {
        t.classList.toggle('active', t.dataset.tab === tab);
    });

    // Bottom nav highlight
    document.querySelectorAll('.cashier-nav-btn').forEach(b => {
        b.classList.toggle('active', b.dataset.tab === tab);
    });
}


/* ============================================================
   4. BADGE UPDATES
   ============================================================ */

function updateBadges() {
    const total = cart.reduce((s, x) => s + x.qty, 0);
    const count = cart.length;

    // Top icon badge
    const topBadge = document.getElementById('cartNavCount');
    if (topBadge) {
        topBadge.textContent = total;
        topBadge.style.display = total > 0 ? 'block' : 'none';
    }

    // Tab badge
    const tabBadge = document.getElementById('tabCartBadge');
    if (tabBadge) {
        tabBadge.textContent = count;
        tabBadge.style.display = count > 0 ? 'inline-block' : 'none';
    }

    // Bottom nav badge
    const navBadge = document.getElementById('navCartBadge');
    if (navBadge) {
        navBadge.textContent = count;
        navBadge.style.display = count > 0 ? 'inline-block' : 'none';
    }
}


/* ============================================================
   5. SCAN BEEP  (Web Audio API, no file needed)
   ============================================================ */

function playScanBeep() {
    try {
        const Ctx = window.AudioContext || window.webkitAudioContext;
        if (!Ctx) return;
        const ctx  = new Ctx();
        const osc  = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.type = 'sine';
        osc.frequency.value = 1040;
        gain.gain.setValueAtTime(0.001, ctx.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.22, ctx.currentTime + 0.01);
        gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.11);
        osc.connect(gain);
        gain.connect(ctx.destination);
        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + 0.12);
    } catch (_) { /* ignore */ }
}


/* ============================================================
   6. SCANNER  (camera)
   ============================================================ */

function setReaderState(state, text) {
    const el = document.getElementById('reader');
    if (!el) return;

    if (state === 'idle') {
        el.innerHTML = `
            <div class="cashier-reader-placeholder">
                <div class="scanner-frame">
                    <div class="scanner-corner tl"></div><div class="scanner-corner tr"></div>
                    <div class="scanner-corner bl"></div><div class="scanner-corner br"></div>
                </div>
                <i class="fa-solid fa-camera fa-2x mb-3" style="color:#94a3b8;"></i>
                <p class="mb-0" style="color:#64748b;font-size:14px;">Tap <b>Open</b> to start scanning</p>
            </div>`;
        el.className = 'cashier-reader';

    } else if (state === 'loading') {
        el.innerHTML = `
            <div class="cashier-reader-placeholder">
                <div class="spinner-border text-primary mb-3" style="width:2.5rem;height:2.5rem;" role="status"></div>
                <p class="mb-0" style="color:#94a3b8;font-size:14px;">Opening camera…</p>
            </div>`;
        el.className = 'cashier-reader';

    } else if (state === 'active') {
        el.className = 'cashier-reader scanner-active-state';
        if (!el.querySelector('.scanner-scan-line')) {
            el.insertAdjacentHTML('beforeend', '<div class="scanner-scan-line"></div>');
        }

    } else if (state === 'success') {
        el.className = 'cashier-reader scanner-success-state';
        el.innerHTML = `
            <div class="cashier-reader-placeholder">
                <i class="fa-solid fa-circle-check fa-3x mb-2" style="color:#22c55e;"></i>
                <p class="mb-0 fw-bold" style="color:#bbf7d0;font-size:14px;">Scanned!</p>
                <small style="color:#86efac;">${text || ''}</small>
            </div>`;
        setTimeout(() => {
            if (scanner) setReaderState('active');
            else         setReaderState('idle');
        }, 900);
    }
}

function startScanner() {
    if (scanner) { toastr.info('Scanner already running'); return; }

    setReaderState('loading');
    isProcessingScan = false;
    lastScannedText  = '';
    lastScanTime     = 0;

    scanner = new Html5Qrcode('reader');

    scanner.start(
        { facingMode: 'environment' },
        { fps: 10, qrbox: { width: 240, height: 120 } },
        function (scannedText) {
            scannedText = (scannedText || '').trim();
            if (!scannedText) return;

            const now = Date.now();
            if (isProcessingScan || (scannedText === lastScannedText && now - lastScanTime < 1800)) return;

            isProcessingScan = true;
            lastScannedText  = scannedText;
            lastScanTime     = now;

            $('#manualSku').val(scannedText);
            playScanBeep();
            if (navigator.vibrate) navigator.vibrate(80);

            setReaderState('success', scannedText);
            fetchProduct(scannedText, true);
        }
    )
    .then(() => {
        setReaderState('active');
        toastr.info('Scanner ready – aim at barcode');
    })
    .catch(() => {
        scanner = null;
        setReaderState('idle');
        toastr.error('Camera permission required');
    });
}

function stopScanner(silent) {
    isProcessingScan = false;
    lastScannedText  = '';
    lastScanTime     = 0;

    if (scanner) {
        scanner.stop()
            .then(() => { try { scanner.clear(); } catch (_) {} scanner = null; setReaderState('idle'); if (!silent) toastr.info('Scanner stopped'); })
            .catch(() => { try { scanner.clear(); } catch (_) {} scanner = null; setReaderState('idle'); if (!silent) toastr.info('Scanner stopped'); });
    } else {
        setReaderState('idle');
    }
}


/* ============================================================
   7. PRODUCT LOOKUP
   ============================================================ */

function fetchProduct(sku, fromScanner) {
    sku = (sku || '').trim();
    if (!sku) { toastr.warning('Enter a product code'); isProcessingScan = false; return; }

    $.get('/CashierPortal/GetProductBySku', { sku })
        .done(function (res) {
            currentProduct       = res.data;
            currentProduct.price = money(currentProduct.price);

            // Populate preview panel
            $('#previewName').text(currentProduct.productName);
            $('#previewSku').text(currentProduct.sku);
            $('#previewCategory').text(currentProduct.categoryName);
            $('#previewPrice').text(fmt(currentProduct.price));
            $('#previewStock').text(currentProduct.stockQty);
            $('#productPreview').removeClass('d-none');

            if (fromScanner) {
                addProductDirectlyToCart(currentProduct);
                // Switch to cart tab briefly then back to scan
                switchTab('cart');
                setTimeout(() => switchTab('scan'), 900);
            } else {
                toastr.success('Product found – tap Add');
            }
        })
        .fail(function (xhr) {
            currentProduct = null;
            $('#productPreview').addClass('d-none');
            toastr.error(xhr.responseJSON?.message || 'Product not found');
        })
        .always(function () {
            setTimeout(() => { isProcessingScan = false; }, 600);
        });
}


/* ============================================================
   8. CART MANAGEMENT
   ============================================================ */

function addProductDirectlyToCart(product) {
    if (!product) return;
    product.price = money(product.price);

    let item = cart.find(x => x.productId === product.productId);
    if (item) {
        if (item.qty + 1 > item.stockQty) { toastr.warning('Not enough stock for ' + item.productName); return; }
        item.qty++;
    } else {
        cart.push({ ...product, qty: 1, price: money(product.price) });
    }

    renderCart();
    updateBadges();
    toastr.success('Added: ' + product.productName);
}

function addToCart() {
    if (!currentProduct) { toastr.warning('Scan or find a product first'); return; }
    addProductDirectlyToCart(currentProduct);
}

function changeQty(productId, delta) {
    let item = cart.find(x => x.productId === productId);
    if (!item) return;

    const newQty = item.qty + delta;
    if (newQty <= 0)              { removeItem(productId); return; }
    if (newQty > item.stockQty)   { toastr.warning('Not enough stock'); return; }

    item.qty = newQty;
    renderCart();
    updateBadges();
}

function removeItem(productId) {
    cart = cart.filter(x => x.productId !== productId);
    renderCart();
    updateBadges();
}


/* ============================================================
   9. TOTALS
   ============================================================ */

function totals() {
    const sub = money(cart.reduce((s, i) => s + money(i.price) * i.qty, 0));
    let   dp  = money($('#discountPercent').val());
    if (dp < 0)   dp = 0;
    if (dp > 100) dp = 100;
    const da  = money((sub * dp) / 100);
    return { sub, discountPercent: dp, discountAmount: da, total: money(sub - da) };
}


/* ============================================================
   10. RENDER CART
   ============================================================ */

function renderCart() {
    const list  = document.getElementById('cartList');
    const empty = document.getElementById('cartEmpty');
    const strip = document.getElementById('cartSummaryStrip');

    if (!list) return;

    // Remove all previous items (keep the empty div)
    list.querySelectorAll('.cart-item').forEach(el => el.remove());

    if (cart.length === 0) {
        if (empty) empty.style.display = 'block';
        if (strip) strip.style.display = 'none';
    } else {
        if (empty) empty.style.display = 'none';
        if (strip) strip.style.display = 'block';

        cart.forEach(function (item) {
            const div = document.createElement('div');
            div.className = 'cart-item';
            div.id = 'cart-item-' + item.productId;
            div.innerHTML = `
                <div class="cart-item-icon"><i class="fa-solid fa-box-open"></i></div>
                <div class="cart-item-info">
                    <div class="cart-item-name">${item.productName}</div>
                    <div class="cart-item-sku">${item.sku}</div>
                    <div class="cart-item-price">₹${fmt(item.price)} each</div>
                </div>
                <div class="cart-item-right">
                    <div class="cart-item-total">₹${fmt(item.price * item.qty)}</div>
                    <div class="qty-ctrl">
                        <button class="qty-remove" onclick="changeQty(${item.productId}, -1)">
                            ${item.qty === 1 ? '<i class="fa-solid fa-trash" style="font-size:11px;"></i>' : '−'}
                        </button>
                        <span class="qty-ctrl-val">${item.qty}</span>
                        <button onclick="changeQty(${item.productId}, 1)">+</button>
                    </div>
                </div>`;
            list.appendChild(div);
        });
    }

    // Update cart summary strip
    const t = totals();
    const si = document.getElementById('cartSummaryItems');
    const sq = document.getElementById('cartSummaryQty');
    if (si) si.textContent = cart.length;
    if (sq) sq.textContent = cart.reduce((s, x) => s + x.qty, 0);

    renderBill();
}


/* ============================================================
   11. RENDER BILL SUMMARY
   ============================================================ */

function renderBill() {
    const t = totals();

    const set = (id, v) => { const el = document.getElementById(id); if (el) el.textContent = v; };

    set('billTotalItems', cart.length);
    set('billTotalQty',   cart.reduce((s, x) => s + x.qty, 0));
    set('subTotal',       '₹' + fmt(t.sub));
    set('discountAmountDisplay', '₹' + fmt(t.discountAmount) + ' (' + fmt(t.discountPercent) + '%)');
    set('grandTotal',     '₹' + fmt(t.total));
}


/* ============================================================
   12. DISCOUNT HELPERS
   ============================================================ */

function adjustDiscount(delta) {
    const field = document.getElementById('discountPercent');
    if (!field) return;
    let val = money(field.value) + delta;
    if (val < 0)   val = 0;
    if (val > 100) val = 100;
    field.value = val;
    renderBill();
}


/* ============================================================
   13. GENERATE BILL
   ============================================================ */

function generateBill() {
    if (cart.length === 0) { toastr.warning('Cart is empty'); return; }

    const t = totals();
    if (t.discountPercent < 0 || t.discountPercent > 100) {
        toastr.error('Discount must be 0–100%');
        return;
    }

    const btn = document.getElementById('btnGenerateBill');
    if (btn) {
        btn.disabled    = true;
        btn.innerHTML   = '<span class="spinner-border spinner-border-sm me-2"></span>Generating…';
    }

    const payload = {
        items: cart.map(item => ({
            productId:   item.productId,
            sku:         item.sku,
            productName: item.productName,
            price:       money(item.price),
            qty:         item.qty
        })),
        discountPercent: t.discountPercent,
        discountAmount:  t.discountAmount
    };

    $.ajax({
        url:         '/CashierPortal/GenerateBill',
        type:        'POST',
        contentType: 'application/json',
        data:        JSON.stringify(payload)
    })
    .done(function (res) {
        toastr.success('Bill ' + res.billNo + ' generated!');
        setTimeout(() => window.open(res.printUrl, '_blank'), 600);

        // Reset
        cart = [];
        $('#discountPercent').val(0);
        renderCart();
        updateBadges();
        switchTab('scan');
    })
    .fail(function (xhr) {
        toastr.error(xhr.responseJSON?.message || 'Bill generation failed');
    })
    .always(function () {
        if (btn) {
            btn.disabled  = false;
            btn.innerHTML = '<i class="fa-solid fa-print me-2"></i>Generate &amp; Print Bill';
        }
    });
}


/* ============================================================
   14. DOM READY
   ============================================================ */

$(function () {

    // Scanner buttons
    $('#btnStartScanner').on('click', startScanner);
    $('#btnStopScanner').on('click', () => stopScanner(false));

    // Manual SKU
    $('#btnFindProduct').on('click', function () {
        fetchProduct($('#manualSku').val(), false);
    });

    $('#manualSku').on('keypress', function (e) {
        if (e.which === 13) fetchProduct($(this).val(), false);
    });

    // Add to cart (manual flow)
    $('#btnAddToCart').on('click', addToCart);

    // Cart nav badge opens cart tab
    $('#btnViewCart').on('click', () => switchTab('cart'));

    // Clear cart
    $('#btnClearCart').on('click', function () {
        if (cart.length === 0) { toastr.info('Cart is already empty'); return; }
        cart = [];
        $('#discountPercent').val(0);
        renderCart();
        updateBadges();
        toastr.info('Cart cleared');
    });

    // Discount input live
    $('#discountPercent').on('input change', renderBill);

    // Generate bill
    $('#btnGenerateBill').on('click', generateBill);

    // Initial render
    renderCart();
    updateBadges();
    switchTab('scan');
});
