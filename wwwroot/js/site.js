/* ============================================================
   SMART BARCODE POS PRO - SITE-WIDE JAVASCRIPT
   ============================================================ */

/* ------------------------------------------------------------
   1. SIDEBAR TOGGLE (mobile)
   ------------------------------------------------------------ */

function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');

    if (!sidebar) return;

    sidebar.classList.toggle('show');

    if (overlay) {
        overlay.classList.toggle('show', sidebar.classList.contains('show'));
    }

    document.body.classList.toggle('sidebar-open', sidebar.classList.contains('show'));
}

function closeSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');

    if (sidebar) sidebar.classList.remove('show');
    if (overlay) overlay.classList.remove('show');

    document.body.classList.remove('sidebar-open');
}

/* ------------------------------------------------------------
   2. SIDEBAR SCROLL PERSISTENCE
      Saves and restores the sidebar scroll position across
      page navigations using localStorage.
   ------------------------------------------------------------ */

document.addEventListener('DOMContentLoaded', function () {

    const sidebar = document.getElementById('sidebar');
    if (!sidebar) return;

    // Restore previous scroll position
    const savedScroll = localStorage.getItem('smart_pos_sidebar_scroll');
    if (savedScroll) {
        sidebar.scrollTop = parseInt(savedScroll, 10);
    }

    // Save scroll position on every scroll event
    sidebar.addEventListener('scroll', function () {
        localStorage.setItem('smart_pos_sidebar_scroll', sidebar.scrollTop);
    });

    // If no saved position, scroll the active menu item into view
    const activeMenu = sidebar.querySelector('a.active');
    if (activeMenu && !savedScroll) {
        activeMenu.scrollIntoView({ block: 'center' });
    }

});

/* ------------------------------------------------------------
   3. DATATABLES INITIALIZATION
      Auto-initializes any <table class="datatable"> element.
      Skips tables that are already initialized.
   ------------------------------------------------------------ */

function initializeDataTables() {

    if (typeof DataTable === 'undefined') return;

    document.querySelectorAll('table.datatable').forEach(function (table) {

        // Skip if already initialized
        if (table.dataset.dtInitialized === 'true') return;

        new DataTable(table, {
            autoWidth:  false,
            pageLength: 10,
            lengthMenu: [10, 25, 50, 100],
            ordering:   true,
            searching:  true,
            language: {
                search:     'Search:',
                lengthMenu: 'Show _MENU_ entries',
                info:       'Showing _START_ to _END_ of _TOTAL_ entries',
                emptyTable: 'No records found',
            },
        });

        table.dataset.dtInitialized = 'true';

    });

}

document.addEventListener('DOMContentLoaded', initializeDataTables);

/* ------------------------------------------------------------
   4. REQUIRED FIELD STARS
      Appends a red asterisk (*) to labels whose matching
      input has the data-val-required attribute.
   ------------------------------------------------------------ */

/* Mobile sidebar auto-close after menu click */
document.addEventListener('DOMContentLoaded', function sidebarMenuLinkAutoClose() {
    const sidebar = document.getElementById('sidebar');
    if (!sidebar) return;

    sidebar.querySelectorAll('a').forEach(function (link) {
        link.addEventListener('click', function () {
            if (window.innerWidth <= 991) {
                closeSidebar();
            }
        });
    });
});

/* Close sidebar on ESC */
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closeSidebar();
    }
});
