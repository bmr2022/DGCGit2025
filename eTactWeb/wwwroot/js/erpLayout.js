(function () {
    function updateErpHeaderClock() {
        const dateEl = document.getElementById("erpHeaderDate");
        const timeEl = document.getElementById("erpHeaderTime");

        if (!dateEl || !timeEl) {
            return false;
        }

        const now = new Date();

        const dateText = new Intl.DateTimeFormat("en-GB", {
            timeZone: "Asia/Kolkata",
            weekday: "long",
            day: "2-digit",
            month: "short",
            year: "numeric"
        }).format(now);

        let timeText = new Intl.DateTimeFormat("en-US", {
            timeZone: "Asia/Kolkata",
            hour: "2-digit",
            minute: "2-digit",
            hour12: true
        }).format(now);

        timeText = timeText.replace(/\b(am|pm)\b/gi, function (value) {
            return value.toUpperCase();
        });

        dateEl.textContent = dateText;
        timeEl.textContent = timeText;

        return true;
    }

    function startErpHeaderClock() {
        updateErpHeaderClock();
        window.setInterval(updateErpHeaderClock, 1000);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", startErpHeaderClock);
    } else {
        startErpHeaderClock();
    }
})();

$(document).ready(function () {
    $(":disabled").css({
        "background-color": "yellow",
        // "border": "2px solid black"
    });
});

/* =========================================================
   XON ERP - CLEAR SIDEBAR SEARCH ON LOGOUT
   ---------------------------------------------------------
   Search is intentionally stored in sessionStorage so it
   survives normal MVC page navigation and refresh.

   On LOGOUT, however, the old search and remembered open
   menu branch must NOT be restored for the next login/session.

   This handler runs in CAPTURE phase so it executes before
   the normal sidebar/navigation click handlers.
   ========================================================= */
(function () {
    "use strict";

    const XON_MENU_SEARCH_KEY = "__XON_ERP_MENU_SEARCH__";
    const XON_SIDEBAR_BRANCH_KEY = "__XON_ERP_OPEN_BRANCH__";

    function clearSidebarSearchOnLogout() {
        try {
            // Clear the search text saved for the previous login.
            sessionStorage.removeItem(XON_MENU_SEARCH_KEY);

            // IMPORTANT: also clear the remembered open menu branch.
            // Otherwise the next user can see the previous user's
            // Reports/Master/Admin/etc. tree already expanded.
            sessionStorage.removeItem(XON_SIDEBAR_BRANCH_KEY);
        } catch (e) {
            console.warn(
                "Unable to clear sidebar state on logout:",
                e
            );
        }

        // Also clear the visible search box immediately, if present.
        const input = document.getElementById("xonMenuSearch");

        if (input) {
            input.value = "";
        }

        const clearButton =
            document.getElementById("xonMenuSearchClear");

        if (clearButton) {
            clearButton.style.display = "none";
        }

        // Close any currently expanded sidebar branches immediately.
        // This prevents the old user's expanded tree from being visible
        // during the logout transition.
        const nav = document.querySelector("#main_nav > .navbar-nav");
        if (nav) {
            nav.querySelectorAll("li.xon-tree-parent").forEach(function (li) {
                li.classList.remove(
                    "xon-tree-open",
                    "xon-tree-click-active",
                    "xon-tree-active-parent",
                    "xon-parent-active",
                    "xon-child-active",
                    "xon-child-child-active"
                );

                const menu = li.querySelector(
                    ":scope > ul.dropdown-menu, :scope > ul.submenu"
                );

                if (menu) {
                    menu.classList.remove("xon-tree-open", "show");
                    menu.setAttribute("aria-hidden", "true");
                }

                const arrow = li.querySelector(
                    ":scope > a > .xon-tree-arrow"
                );

                if (arrow) {
                    arrow.textContent = "+";
                }
            });
        }
    }

    document.addEventListener(
        "click",
        function (e) {
            const target =
                e.target && e.target.closest
                    ? e.target.closest(
                        "#main_nav .Logout, " +
                        "#main_nav a.Logout, " +
                        "#main_nav .Logout a"
                    )
                    : null;

            if (!target) {
                return;
            }

            clearSidebarSearchOnLogout();
        },
        true
    );

    /*
     * Some pages may identify Logout by its rendered label instead
     * of the .Logout class. Handle that case as well without
     * affecting normal menu navigation.
     */
    document.addEventListener(
        "click",
        function (e) {
            // The header Logout button is outside #main_nav on the
            // dashboard (as shown in the UI), so do NOT restrict this
            // check to the sidebar.
            const link =
                e.target && e.target.closest
                    ? e.target.closest("a, button, [role=\"button\"]")
                    : null;

            if (!link) {
                return;
            }

            const text = (link.textContent || "")
                .replace(/\s+/g, " ")
                .trim()
                .toLowerCase();

            // Match the actual header Logout control even when it is
            // outside the sidebar/menu DOM.
            if (text === "logout") {
                clearSidebarSearchOnLogout();
            }
        },
        true
    );

    // Expose it in case the Razor/MVC logout code wants to call it
    // explicitly before redirecting.
    window.xonClearSidebarSearchOnLogout =
        clearSidebarSearchOnLogout;
})();


/* Bootstrap dropdown/submenu click handling is intentionally disabled here.
 * The XON tree handler below owns sidebar parent/child expansion.
 */

function toggleErpSidebar() {
    const sidebar = document.getElementById("main_nav");

    if (!sidebar) return;

    sidebar.classList.toggle("erp-sidebar-collapsed");
    document.body.classList.toggle("erp-sidebar-collapsed");

    const collapsed = sidebar.classList.contains("erp-sidebar-collapsed");
    const button = document.querySelector("header .navbar-toggler");

    if (button) {
        button.setAttribute("aria-expanded", (!collapsed).toString());
    }
}

(function () {
    "use strict";

    const PIN_STORAGE_KEY = "xon_sidebar_pinned";

    function getSidebar() {
        return document.getElementById("main_nav");
    }

    function getPinnedState() {
        return localStorage.getItem(PIN_STORAGE_KEY) === "true";
    }

    function setSidebarState(expanded) {
        const sidebar = getSidebar();

        if (!sidebar) {
            return false;
        }

        const collapsed = !expanded;

        sidebar.classList.toggle(
            "erp-sidebar-collapsed",
            collapsed
        );

        document.body.classList.toggle(
            "erp-sidebar-collapsed",
            collapsed
        );

        const button =
            document.querySelector("header .navbar-toggler");

        if (button) {
            button.setAttribute(
                "aria-expanded",
                expanded.toString()
            );
        }

        return true;
    }

    function updatePinUI(pinned) {
        const pin = document.getElementById("xonMenuPin");

        if (!pin) {
            return;
        }

        pin.classList.toggle("is-pinned", pinned);
        pin.setAttribute("aria-pressed", pinned.toString());
        pin.setAttribute(
            "title",
            pinned ? "Unpin Sidebar" : "Pin Sidebar"
        );
        pin.setAttribute(
            "aria-label",
            pinned ? "Unpin Sidebar" : "Pin Sidebar"
        );
    }

    window.xonUpdateSidebarPinUI = updatePinUI;

    function ensurePinButton() {
        const searchBox =
            document.getElementById("xonMenuSearchBox");

        if (!searchBox) {
            return null;
        }

        const searchInner =
            searchBox.querySelector(".xon-menu-search-inner");

        if (!searchInner) {
            return null;
        }

        let row =
            searchBox.querySelector(".xon-search-row");

        if (!row) {
            row = document.createElement("div");
            row.className = "xon-search-row";
            searchInner.parentNode.insertBefore(row, searchInner);
            row.appendChild(searchInner);
        }

        let pin =
            row.querySelector("#xonMenuPin");

        if (!pin) {
            pin = document.createElement("button");
            pin.type = "button";
            pin.id = "xonMenuPin";
            pin.className = "xon-menu-pin";
            pin.innerHTML = '<i class="fas fa-thumbtack" aria-hidden="true"></i>';
            row.appendChild(pin);
        }

        /* ---------------------------------------------------------
           Expand / Collapse All button.
           Menu names are NOT hardcoded here. All menus continue to
           come from the database and are handled from the rendered DOM.
           --------------------------------------------------------- */
        let expandButton =
            row.querySelector("#xonMenuExpandAll");

        if (!expandButton) {
            expandButton = document.createElement("button");
            expandButton.type = "button";
            expandButton.id = "xonMenuExpandAll";
            expandButton.className = "xon-menu-expand-all";
            expandButton.title = "Expand All Menus";
            expandButton.setAttribute(
                "aria-label",
                "Expand All Menus"
            );
            expandButton.setAttribute(
                "aria-pressed",
                "false"
            );
            expandButton.innerHTML =
                '<i class="fas fa-angles-down" aria-hidden="true"></i>';

            pin.insertAdjacentElement(
                "afterend",
                expandButton
            );
        }

        return pin;
    }

    function getSidebarTreeParents() {
        const nav = document.querySelector(
            "#main_nav > .navbar-nav"
        );

        if (!nav) {
            return [];
        }

        return Array.from(
            nav.querySelectorAll("li.xon-tree-parent")
        );
    }

    function setSidebarTreeMenuOpen(li, open) {
        if (!li) {
            return;
        }

        const menu = li.querySelector(
            ":scope > ul.dropdown-menu, " +
            ":scope > ul.submenu"
        );

        if (!menu) {
            return;
        }

        li.classList.toggle(
            "xon-tree-open",
            open
        );

        li.classList.toggle(
            "xon-tree-click-active",
            open
        );

        li.classList.remove(
            "xon-tree-active-parent"
        );

        menu.classList.toggle(
            "xon-tree-open",
            open
        );

        menu.classList.remove("show");

        menu.setAttribute(
            "aria-hidden",
            open ? "false" : "true"
        );

        const arrow = li.querySelector(
            ":scope > a > .xon-tree-arrow"
        );

        if (arrow) {
            arrow.textContent = open ? "−" : "+";
        }
    }

    function getSidebarSearchQuery() {
        const input =
            document.getElementById("xonMenuSearch");

        return input
            ? (input.value || "").trim().toLowerCase()
            : "";
    }

    function setExpandCollapseButtonState(expanded, searchMode) {
        const button =
            document.getElementById("xonMenuExpandAll");

        if (!button) return;

        button.setAttribute(
            "aria-pressed",
            expanded ? "true" : "false"
        );

        if (searchMode) {
            button.title = expanded
                ? "Collapse to Search Results"
                : "Expand All Menus";
            button.setAttribute(
                "aria-label",
                expanded
                    ? "Collapse to Search Results"
                    : "Expand All Menus"
            );
        } else {
            button.title = expanded
                ? "Collapse All Menus"
                : "Expand All Menus";
            button.setAttribute(
                "aria-label",
                expanded
                    ? "Collapse All Menus"
                    : "Expand All Menus"
            );
        }

        const icon = button.querySelector("i");
        if (icon) {
            icon.className = expanded
                ? "fas fa-angles-up"
                : "fas fa-angles-down";
        }
    }

    function expandAllSidebarMenus() {
        const parents = getSidebarTreeParents();

        parents.forEach(function (li) {
            setSidebarTreeMenuOpen(li, true);
        });

        setExpandCollapseButtonState(
            true,
            !!getSidebarSearchQuery()
        );
    }

    function collapseAllSidebarMenus() {
        const parents = getSidebarTreeParents();

        parents.forEach(function (li) {
            setSidebarTreeMenuOpen(li, false);
        });

        setExpandCollapseButtonState(false, false);
    }

    /*
     * Search mode needs a special collapse operation.
     * We must NOT leave a matching database menu hidden.
     * First close every branch, then reopen only the ancestors
     * required to display the current search matches.
     */
    function collapseToSearchResults() {
        const nav = document.querySelector(
            "#main_nav > .navbar-nav"
        );

        if (!nav) return;

        const parents = getSidebarTreeParents();

        parents.forEach(function (li) {
            setSidebarTreeMenuOpen(li, false);
        });

        nav.querySelectorAll("li.xon-search-match")
            .forEach(function (match) {
                match.classList.remove("xon-search-hidden");

                let current = match;

                while (current && nav.contains(current)) {
                    const parent =
                        current.parentElement
                            ? current.parentElement.closest(
                                "li.xon-tree-parent"
                            )
                            : null;

                    if (!parent) break;

                    parent.classList.remove("xon-search-hidden");
                    setSidebarTreeMenuOpen(parent, true);
                    current = parent;
                }
            });

        setExpandCollapseButtonState(true, true);
    }

    function bindExpandCollapseButton() {
        const button =
            document.getElementById("xonMenuExpandAll");

        if (!button) {
            return false;
        }

        if (button.dataset.xonExpandBound === "true") {
            return true;
        }

        button.dataset.xonExpandBound = "true";

        button.addEventListener(
            "click",
            function (e) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();

                const parents = getSidebarTreeParents();

                if (!parents.length) {
                    return;
                }

                const searchQuery =
                    getSidebarSearchQuery();

                const allOpen = parents.every(function (li) {
                    return li.classList.contains(
                        "xon-tree-open"
                    );
                });

                if (searchQuery) {
                    if (allOpen) {
                        /*
                         * Search is active: second click returns to
                         * the minimum tree needed to show matches.
                         */
                        collapseToSearchResults();
                    } else {
                        /* First click after searching: show all DB menus. */
                        expandAllSidebarMenus();
                    }
                } else if (allOpen) {
                    collapseAllSidebarMenus();
                } else {
                    expandAllSidebarMenus();
                }
            },
            true
        );

        return true;
    }

    function bindPinButton() {
        const pin = ensurePinButton();

        if (!pin) {
            return false;
        }

        bindExpandCollapseButton();

        const pinned = getPinnedState();
        updatePinUI(pinned);

        if (pin.dataset.xonPinBound === "true") {
            return true;
        }

        pin.dataset.xonPinBound = "true";

        pin.addEventListener("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();

            const newPinned = !getPinnedState();

            localStorage.setItem(
                PIN_STORAGE_KEY,
                newPinned.toString()
            );

            setSidebarState(newPinned);
            updatePinUI(newPinned);

            const sidebar = getSidebar();

            if (sidebar) {
                sidebar.classList.remove(
                    "erp-sidebar-hover-expanded"
                );
            }

            document.body.classList.remove(
                "erp-sidebar-hover-expanded"
            );
        }, true);

        return true;
    }

    function applySavedSidebarState() {
        /*
         * First visit keeps the existing ERP behaviour:
         * Dashboard expanded, other pages collapsed.
         */
        if (localStorage.getItem(PIN_STORAGE_KEY) === null) {
            const currentPath = window.location.pathname
                .toLowerCase()
                .replace(/\/+$/, "");

            const isDashboard =
                currentPath === "/home/dashboard" ||
                currentPath === "/home" ||
                currentPath === "/";

            setSidebarState(isDashboard);
            updatePinUI(false);
            return true;
        }

        const pinned = getPinnedState();
        setSidebarState(pinned);
        updatePinUI(pinned);
        return true;
    }

    function initSidebarPinState() {
        bindPinButton();
        applySavedSidebarState();

        [0, 50, 150, 300, 500, 800].forEach(function (delay) {
            window.setTimeout(function () {
                bindPinButton();
                applySavedSidebarState();
            }, delay);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            initSidebarPinState
        );
    } else {
        initSidebarPinState();
    }

    window.addEventListener("pageshow", function () {
        window.setTimeout(function () {
            bindPinButton();
            applySavedSidebarState();
        }, 0);
    });
})();

/* =========================================================
   XON ERP - PIN BUTTON VISUAL STATE
   Teal = PINNED | Light red = UNPINNED
   ========================================================= */
(function () {
    "use strict";

    function installPinStyle() {
        if (document.getElementById("xon-sidebar-pin-style")) {
            return;
        }

        const style = document.createElement("style");
        style.id = "xon-sidebar-pin-style";
        style.textContent = `
            #main_nav #xonMenuSearchBox .xon-search-row {
                display: flex !important;
                align-items: center !important;
                width: 100% !important;
                gap: 6px !important;
            }

            #main_nav #xonMenuSearchBox .xon-search-row .xon-menu-search-inner {
                flex: 1 1 auto !important;
                min-width: 0 !important;
                width: auto !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-pin {
                width: 32px !important;
                min-width: 32px !important;
                height: 40px !important;
                flex: 0 0 32px !important;
                display: flex !important;
                align-items: center !important;
                justify-content: center !important;
                padding: 0 !important;
                margin: 0 !important;
                border: 1px solid #d6e3e4 !important;
                border-radius: 8px !important;
                background: #fff5f5 !important;
                color: #dc5a5a !important;
                cursor: pointer !important;
                transition: all .18s ease !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-pin:hover {
                background: #ffe7e7 !important;
                border-color: #dc5a5a !important;
                color: #c73f3f !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-pin.is-pinned {
                background: #075b60 !important;
                border-color: #075b60 !important;
                color: #fff !important;
                box-shadow: 0 2px 7px rgba(7,91,96,.20) !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-pin.is-pinned:hover {
                background: #03484d !important;
                border-color: #03484d !important;
                color: #fff !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-pin i {
                font-size: 13px !important;
                pointer-events: none !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-expand-all {
                width: 32px !important;
                min-width: 32px !important;
                height: 40px !important;
                flex: 0 0 32px !important;
                display: flex !important;
                align-items: center !important;
                justify-content: center !important;
                padding: 0 !important;
                margin: 0 !important;
                border: 1px solid #d6e3e4 !important;
                border-radius: 8px !important;
                background: #ffffff !important;
                color: #075b60 !important;
                cursor: pointer !important;
                transition: all .18s ease !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-expand-all:hover {
                background: #eaf4f4 !important;
                border-color: #075b60 !important;
                color: #03484d !important;
            }

            #main_nav #xonMenuSearchBox .xon-menu-expand-all i {
                font-size: 13px !important;
                pointer-events: none !important;
            }
        `;

        document.head.appendChild(style);
    }

    function initPinStyle() {
        installPinStyle();

        const pinned =
            localStorage.getItem("xon_sidebar_pinned") === "true";

        if (typeof window.xonUpdateSidebarPinUI === "function") {
            window.xonUpdateSidebarPinUI(pinned);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initPinStyle);
    } else {
        initPinStyle();
    }
})();

(function () {

    const labels = {
        'ADMIN': { text: 'ADMIN', icon: 'fa-gear' },
        'MASTER': { text: 'MASTER', icon: 'fa-boxes-stacked' },
        'SALES & MKT': { text: 'SALES & MKT', icon: 'fa-chart-column' },
        'PROCURMENT MGMT': { text: 'PROCURMENT MGMT', icon: 'fa-cart-shopping' },
        'MATERIAL MGMT': { text: 'MATERIAL MGMT', icon: 'fa-box-open' },
        'PRODUCTION & PLANNING': { text: 'PRODUCTION & PLANNING', icon: 'fa-industry' },
        'QUALITY MGMT': { text: 'QUALITY MGMT', icon: 'fa-circle-check' },
        'FINANCIAL ACCOUNTING': { text: 'FINANCIAL ACCOUNTING', icon: 'fa-coins' },
        'P & M': { text: 'P & M', icon: 'fa-screwdriver-wrench' },
        'R & D': { text: 'R & D', icon: 'fa-diagram-project' },
        'HR': { text: 'HR', icon: 'fa-users' },
        'REPORTS': { text: 'REPORTS', icon: 'fa-file-lines' },
        'GENERAL': { text: 'GENERAL', icon: 'fa-shield-halved' },
        'LOGOUT': {
            text: 'Logout',
            icon: 'fa-right-from-bracket',
            style: 'color: red !important;'
        }
    };

    function applyMenu(a, def) {
        if (def.text === '__HIDE__') {
            a.closest('.nav-item')?.classList.add('mis-hidden-item');
            return;
        }

        // Keep the icon, but remove old text nodes / generated labels.
        a.querySelectorAll(':scope>.mis-label').forEach(function (x) {
            x.remove();
        });

        Array.from(a.childNodes).forEach(function (node) {
            if (node.nodeType === Node.TEXT_NODE) {
                node.remove();
            }
        });

        var icon = a.querySelector(':scope>i');

        if (!icon) {
            icon = document.createElement('i');
            a.insertBefore(icon, a.firstChild);
        }

        icon.className = 'fas ' + def.icon;

        var label = document.createElement('span');
        label.className = 'mis-label';
        label.textContent = def.text;
        a.appendChild(label);

        // Visible label = compact/hard-coded DisplayName.
        // Hover tooltip = DB Mainmenuheading.
        const dbMainMenuHeading =
            (a.getAttribute('data-main-menu-heading') || '').trim();

        if (dbMainMenuHeading) {
            a.setAttribute('data-full-menu-name', dbMainMenuHeading);
        } else {
            a.removeAttribute('data-full-menu-name');
        }

        a.removeAttribute('title');

        // Keep Logout styling isolated from the normal menu active/hover styles.
        const navItem = a.closest('.nav-item');
        if (navItem) {
            navItem.classList.toggle('Logout', def.text === 'Logout');
        }

        if (def.text === 'Logout') {
            a.style.setProperty('color', '#dc3545', 'important');
            icon.style.setProperty('color', '#dc3545', 'important');
        }


    }

    function ensureDashboardMenuStyles() {
        if (document.getElementById("xon-dashboard-menu-styles")) {
            return;
        }

        const style = document.createElement("style");
        style.id = "xon-dashboard-menu-styles";

        style.textContent = `
            #main_nav .mis-dashboard-submenu {
                display: none;
                width: 100%;
                margin: 4px 0 8px !important;
                padding: 7px !important;
                box-sizing: border-box;
                background: #f7fbfb !important;
                border: 1px solid #e3eeee !important;
                border-radius: 8px !important;
            }

            #main_nav .nav-item.mis-dashboard-expanded > .mis-dashboard-submenu {
                display: flex !important;
                flex-direction: column !important;
                gap: 3px !important;
            }

            #main_nav .mis-dashboard-child {
                width: 100% !important;
                min-height: 34px !important;
                display: flex !important;
                align-items: center !important;
                gap: 8px !important;
                padding: 4px 7px !important;
                margin: 0 !important;
                box-sizing: border-box !important;
                border: 0 !important;
                border-radius: 6px !important;
                outline: 0 !important;
                background: transparent !important;
                color: #526166 !important;
                font-family: Arial, Helvetica, sans-serif !important;
                font-size: 11px !important;
                font-weight: 700 !important;
                line-height: 1.2 !important;
                text-align: left !important;
                white-space: nowrap !important;
                cursor: pointer !important;
                appearance: none !important;
                -webkit-appearance: none !important;
                box-shadow: none !important;
            }

            #main_nav .mis-dashboard-child i {
                width: 25px !important;
                height: 25px !important;
                min-width: 25px !important;
                margin: 0 !important;
                display: inline-flex !important;
                align-items: center !important;
                justify-content: center !important;
                border-radius: 6px !important;
                background: #edf7f7 !important;
                color: #075b60 !important;
                font-size: 11px !important;
            }

            #main_nav .mis-dashboard-child:hover {
                background: #eaf4f4 !important;
                color: #075b60 !important;
            }

            #main_nav .mis-dashboard-child.active {
                background: #e1eeee !important;
                color: #03484d !important;
                font-weight: 600 !important;
            }

            #main_nav .mis-dashboard-child.active i {
                background: #075b60 !important;
                color: #fff !important;
            }

            #main_nav .mis-dashboard-parent {
                position: relative !important;
            }

            #main_nav .mis-dashboard-parent::after {
                content: "\\f078" !important;
                font-family: "Font Awesome 6 Free" !important;
                font-weight: 900 !important;
                margin-left: auto !important;
                color: rgba(255,255,255,.80) !important;
                font-size: 9px !important;
                transition: transform .2s ease !important;
            }

            #main_nav .nav-item.mis-dashboard-expanded > .mis-dashboard-parent::after {
                transform: rotate(180deg) !important;
            }

            #main_nav .nav-item.mis-dashboard-expanded > .mis-dashboard-parent {
                background: linear-gradient(135deg, #09656a, #034f54) !important;
                color: #fff !important;
                box-shadow: 0 4px 12px rgba(4,79,84,.18) !important;
            }

            #main_nav .nav-item.mis-dashboard-expanded > .mis-dashboard-parent > i {
                background: rgba(255,255,255,.16) !important;
                color: #fff !important;
            }

            @media (max-width: 700px) {
                #main_nav .mis-dashboard-submenu {
                    padding: 5px !important;
                }

                #main_nav .mis-dashboard-child {
                    min-height: 31px !important;
                    font-size: 10px !important;
                }
            }
        `;

        document.head.appendChild(style);
    }


    /* =========================================================
       XON ERP - HIERARCHICAL ACTIVE STATE
       Level 1 = top parent
       Level 2 = child parent
       Level 3 = leaf / child-child
       ========================================================= */
    window.xonSetHierarchicalActive = function (clickedLink) {

        const nav = document.querySelector("#main_nav > .navbar-nav");

        if (!nav || !clickedLink || !nav.contains(clickedLink)) {
            return;
        }

        const activeClasses = [
            "xon-parent-active",
            "xon-child-active",
            "xon-child-child-active"
        ];

        /* Clear only our custom hierarchy classes. */
        nav.querySelectorAll("a").forEach(function (link) {
            activeClasses.forEach(function (className) {
                link.classList.remove(className);
            });
        });

        /* Find the top-level parent. */
        const topItem =
            clickedLink.closest("#main_nav > .navbar-nav > .nav-item");

        if (!topItem) {
            return;
        }

        const topLink =
            topItem.querySelector(":scope > .nav-link");

        if (topLink) {
            topLink.classList.add(
                "active",
                "xon-parent-active"
            );
        }

        /*
         * Build the exact LI path:
         *
         * ADMIN
         *   User
         *     New User
         *
         * path = [ADMIN li, User li, New User li]
         */
        const clickedLi = clickedLink.closest("li");

        if (!clickedLi) {
            return;
        }

        const path = [];
        let current = clickedLi;

        while (current && nav.contains(current)) {

            path.unshift(current);

            if (current === topItem) {
                break;
            }

            current =
                current.parentElement &&
                current.parentElement.closest("li");
        }

        if (path[0] !== topItem) {
            return;
        }

        /*
         * Apply classes by real depth, not by click type.
         *
         * depth 1 = ADMIN
         * depth 2 = User
         * depth 3 = New User
         */
        path.forEach(function (li, index) {

            const link =
                li === topItem
                    ? topLink
                    : li.querySelector(":scope > a");

            if (!link) {
                return;
            }

            link.classList.add("active");

            if (index === 0) {
                link.classList.add("xon-parent-active");
            }
            else if (index === path.length - 1 && index >= 2) {
                link.classList.add("xon-child-child-active");
            }
            else {
                link.classList.add("xon-child-active");
            }
        });

        /* Update breadcrumb immediately after menu click. */
        if (typeof window.xonUpdateBreadcrumb === "function") {
            window.xonUpdateBreadcrumb();
        }
    };

    function getDashboardNameFromHash() {

        const hash =
            (window.location.hash || '')
                .replace(/^#/, '')
                .trim()
                .toLowerCase();

        const hashMap = {
            'inventorytab': 'Inventory Dashboard',
            'purchasetab': 'Purchase Dashboard',
            'salestab': 'Sales Dashboard',
            'productiontab': 'Production Dashboard',
            'mistab': 'MIS Dashboard'
        };

        return hashMap[hash] || '';
    }

    function getDashboardChildByName(dashboardSubmenu, dashboardName) {

        if (!dashboardSubmenu || !dashboardName) {
            return null;
        }

        const wanted = dashboardName
            .replace(/\s+/g, ' ')
            .trim()
            .toLowerCase();

        return Array.from(
            dashboardSubmenu.querySelectorAll('.mis-dashboard-child')
        ).find(function (child) {

            const name = (
                child.getAttribute('data-dashboard-tab') || ''
            )
                .replace(/\s+/g, ' ')
                .trim()
                .toLowerCase();

            return name === wanted;
        }) || null;
    }

    function getDashboardTopTabByName(dashboardName) {

        if (!dashboardName) {
            return null;
        }

        const wanted = dashboardName
            .replace(/\s+/g, ' ')
            .trim()
            .toLowerCase();

        const elements = document.querySelectorAll(
            'button, a, [role="tab"], .nav-link, .dashboard-tab'
        );

        return Array.from(elements).find(function (element) {

            /* Do not treat generated sidebar buttons as dashboard tabs. */
            if (element.classList.contains('mis-dashboard-child')) {
                return false;
            }

            if (element.closest('#main_nav')) {
                return false;
            }

            const text = (element.textContent || '')
                .replace(/\s+/g, ' ')
                .trim()
                .toLowerCase();

            return text === wanted;
        }) || null;
    }

    function updateDashboardBreadcrumb() {

        const breadcrumb =
            document.getElementById('xonBreadcrumb');

        if (!breadcrumb) {
            return false;
        }

        /*
         * IMPORTANT:
         * URL hash has first priority.
         * Therefore #salesTab can never incorrectly show Inventory.
         */
        let dashboardName = getDashboardNameFromHash();

        /* Fallback 1: active sidebar dashboard child. */
        if (!dashboardName) {

            const activeDashboard =
                document.querySelector(
                    '#main_nav .mis-dashboard-child.active'
                );

            if (activeDashboard) {
                dashboardName =
                    activeDashboard.getAttribute(
                        'data-dashboard-tab'
                    ) ||
                    (activeDashboard.textContent || '')
                        .replace(/\s+/g, ' ')
                        .trim();
            }
        }

        /* Fallback 2: active real dashboard tab. */
        if (!dashboardName) {

            const dashboardNames = [
                'Inventory Dashboard',
                'Purchase Dashboard',
                'Sales Dashboard',
                'Production Dashboard',
                'MIS Dashboard'
            ];

            const activeTab =
                Array.from(
                    document.querySelectorAll(
                        'button, a, [role="tab"], .nav-link, .dashboard-tab'
                    )
                ).find(function (element) {

                    if (element.classList.contains('mis-dashboard-child')) {
                        return false;
                    }

                    if (element.closest('#main_nav')) {
                        return false;
                    }

                    if (!element.classList.contains('active')) {
                        return false;
                    }

                    const text = (element.textContent || '')
                        .replace(/\s+/g, ' ')
                        .trim()
                        .toLowerCase();

                    return dashboardNames.some(function (name) {
                        return name.toLowerCase() === text;
                    });
                });

            if (activeTab) {
                dashboardName = (activeTab.textContent || '')
                    .replace(/\s+/g, ' ')
                    .trim();
            }
        }

        if (!dashboardName) {
            breadcrumb.innerHTML = '';
            breadcrumb.classList.remove('is-visible');
            return false;
        }

        breadcrumb.innerHTML = '';

        /* Dashboard */
        const parent = document.createElement('span');
        parent.className = 'xon-breadcrumb-item';

        const parentIcon = document.createElement('i');
        parentIcon.className = 'fas fa-gauge-high';
        parentIcon.setAttribute('aria-hidden', 'true');
        parent.appendChild(parentIcon);

        const parentText = document.createElement('span');
        parentText.textContent = 'Dashboard';
        parent.appendChild(parentText);

        breadcrumb.appendChild(parent);

        /* Separator */
        const separator = document.createElement('span');
        separator.className = 'xon-breadcrumb-separator';
        separator.textContent = '›';
        breadcrumb.appendChild(separator);

        /* Current dashboard */
        const child = document.createElement('span');
        child.className = 'xon-breadcrumb-item current';

        const dashboardSubmenu = document.querySelector(
            '#main_nav .mis-dashboard-submenu'
        );

        const activeDashboardChild =
            getDashboardChildByName(
                dashboardSubmenu,
                dashboardName
            );

        if (activeDashboardChild) {

            const originalIcon =
                activeDashboardChild.querySelector('i');

            if (originalIcon) {
                const childIcon = document.createElement('i');
                childIcon.className = originalIcon.className;
                childIcon.setAttribute('aria-hidden', 'true');
                child.appendChild(childIcon);
            }
        }

        const childText = document.createElement('span');
        childText.textContent = dashboardName;
        child.appendChild(childText);

        breadcrumb.appendChild(child);
        breadcrumb.classList.add('is-visible');

        return true;
    }

    window.xonUpdateBreadcrumb = function () {

        if (updateDashboardBreadcrumb()) {
            return;
        }

        const breadcrumb =
            document.getElementById("xonBreadcrumb");

        const nav =
            document.querySelector("#main_nav > .navbar-nav");

        if (!breadcrumb || !nav) {
            return;
        }

        const links = [];

        const parentLink =
            nav.querySelector(
                "a.xon-parent-active"
            );

        if (parentLink) {
            links.push(parentLink);
        }

        const childLink =
            nav.querySelector(
                "a.xon-child-active"
            );

        if (childLink) {
            links.push(childLink);
        }

        const childChildLink =
            nav.querySelector(
                "a.xon-child-child-active"
            );

        if (childChildLink) {
            links.push(childChildLink);
        }

        /*
         * Do not show an empty breadcrumb.
         */
        if (!links.length) {
            breadcrumb.innerHTML = "";
            breadcrumb.classList.remove("is-visible");
            return;
        }

        breadcrumb.innerHTML = "";

        links.forEach(function (link, index) {

            const text =
                (link.textContent || "")
                    .replace(/\s+/g, " ")
                    .trim();

            if (!text) {
                return;
            }

            if (index > 0) {

                const separator =
                    document.createElement("span");

                separator.className =
                    "xon-breadcrumb-separator";

                separator.textContent =
                    "›";

                breadcrumb.appendChild(separator);
            }

            const item =
                document.createElement("span");

            item.className =
                "xon-breadcrumb-item";

            if (index === links.length - 1) {
                item.classList.add("current");
            }

            /*
             * Use the SAME icon as the sidebar menu.
             */
            const sourceIcon =
                link.querySelector(
                    ":scope > i:not(.xon-tree-arrow)"
                ) ||
                link.querySelector(
                    "i.xon-child-icon"
                ) ||
                link.querySelector(
                    "i"
                );

            if (sourceIcon) {

                const breadcrumbIcon =
                    document.createElement("i");

                breadcrumbIcon.className =
                    sourceIcon.className;

                breadcrumbIcon.setAttribute(
                    "aria-hidden",
                    "true"
                );

                item.appendChild(
                    breadcrumbIcon
                );
            }

            const textSpan =
                document.createElement("span");

            textSpan.textContent =
                text;

            item.appendChild(
                textSpan
            );

            breadcrumb.appendChild(
                item
            );
        });

        breadcrumb.classList.add("is-visible");
    };

    function initXonBreadcrumb() {

        /*
         * restoreActiveTree() is called later in this script,
         * so wait until the menu has finished initializing.
         */
        window.setTimeout(function () {

            if (typeof window.xonUpdateBreadcrumb === "function") {
                window.xonUpdateBreadcrumb();
            }

        }, 350);
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            initXonBreadcrumb
        );
    } else {
        initXonBreadcrumb();
    }

    /*
     * =========================================================
     * XON ERP - CURRENT FORM KEY
     * ---------------------------------------------------------
     * Priority:
     *   1. URL query string: ?formKey=...
     *   2. data-form-key / data-formkey on main_nav/body
     *   3. hidden input name="formKey"
     *   4. meta name="formKey"
     *
     * This FormKey is sent to UserFavouriteMenuController so
     * the controller can resolve UID_{formKey} from Session.
     * =========================================================
     */
    function getCurrentFormKey() {
        try {
            const params = new URLSearchParams(window.location.search);

            const queryFormKey =
                params.get("formKey") ||
                params.get("FormKey") ||
                params.get("FORMKEY");

            if (queryFormKey && queryFormKey.trim()) {
                return queryFormKey.trim();
            }

            const elements = [
                document.querySelector("#main_nav"),
                document.body,
                document.documentElement
            ];

            for (const element of elements) {
                if (!element) continue;

                const dataFormKey =
                    element.getAttribute("data-form-key") ||
                    element.getAttribute("data-formkey");

                if (dataFormKey && dataFormKey.trim()) {
                    return dataFormKey.trim();
                }
            }

            const hiddenInput =
                document.querySelector(
                    'input[name="formKey"], input[name="FormKey"], input[id="formKey"], input[id="FormKey"]'
                );

            if (hiddenInput && hiddenInput.value && hiddenInput.value.trim()) {
                return hiddenInput.value.trim();
            }

            const meta =
                document.querySelector(
                    'meta[name="formKey"], meta[name="FormKey"]'
                );

            if (meta) {
                const metaFormKey = meta.getAttribute("content");

                if (metaFormKey && metaFormKey.trim()) {
                    return metaFormKey.trim();
                }
            }

            return "";
        }
        catch (error) {
            console.error("Unable to read current FormKey:", error);
            return "";
        }
    }

    /*
     * =============================================================
     * XON ERP - MY FAVOURITES TOGGLE
     * =============================================================
     *
     * Use ONE document-level capture handler.
     *
     * Reason:
     * The sidebar has multiple click handlers and some of them are
     * attached dynamically. A delegated capture handler guarantees
     * that clicking MY FAVOURITES is handled before the normal
     * sidebar/tree handlers.
     */

    function installFavouriteToggle() {

        if (
            document.documentElement.dataset
                .xonFavouriteToggleBound ===
            "true"
        ) {
            return;
        }

        document.documentElement.dataset
            .xonFavouriteToggleBound =
            "true";


        document.addEventListener(
            "click",
            function (e) {

                const header =
                    e.target.closest
                        ? e.target.closest(
                            "#main_nav .xon-favourites-header"
                        )
                        : null;

                if (!header) {
                    return;
                }

                const container =
                    header.closest(
                        ".xon-favourites-item"
                    );

                if (!container) {
                    return;
                }

                const list =
                    container.querySelector(
                        ":scope > .xon-favourites-list"
                    );

                const chevron =
                    container.querySelector(
                        ":scope > .xon-favourites-header .xon-favourites-chevron"
                    );

                if (!list) {
                    return;
                }

                /*
                 * Stop every other sidebar click handler.
                 */
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();


                const isCollapsed =
                    container.classList.contains(
                        "is-collapsed"
                    );


                if (isCollapsed) {

                    /*
                     * =============================
                     * OPEN
                     * =============================
                     */

                    container.classList.remove(
                        "is-collapsed"
                    );

                    list.style.setProperty(
                        "display",
                        "block",
                        "important"
                    );

                    list.style.setProperty(
                        "visibility",
                        "visible",
                        "important"
                    );

                    list.style.setProperty(
                        "opacity",
                        "1",
                        "important"
                    );

                    list.style.setProperty(
                        "max-height",
                        "10000px",
                        "important"
                    );

                    header.setAttribute(
                        "aria-expanded",
                        "true"
                    );

                    if (chevron) {
                        chevron.textContent = "▴";
                    }

                } else {

                    /*
                     * =============================
                     * CLOSE
                     * =============================
                     */

                    container.classList.add(
                        "is-collapsed"
                    );

                    list.style.setProperty(
                        "display",
                        "none",
                        "important"
                    );

                    list.style.setProperty(
                        "visibility",
                        "hidden",
                        "important"
                    );

                    list.style.setProperty(
                        "opacity",
                        "0",
                        "important"
                    );

                    list.style.setProperty(
                        "max-height",
                        "0",
                        "important"
                    );

                    header.setAttribute(
                        "aria-expanded",
                        "false"
                    );

                    if (chevron) {
                        chevron.textContent = "▾";
                    }
                }

                return false;
            },
            true
        );


        /*
         * Keyboard support.
         */
        document.addEventListener(
            "keydown",
            function (e) {

                if (
                    e.key !== "Enter" &&
                    e.key !== " "
                ) {
                    return;
                }

                const header =
                    e.target.closest
                        ? e.target.closest(
                            "#main_nav .xon-favourites-header"
                        )
                        : null;

                if (!header) {
                    return;
                }

                e.preventDefault();

                /*
                 * Trigger the same delegated click logic.
                 */
                header.click();

            },
            true
        );
    }

    function setupFavouriteMenus(nav) {
        if (!nav) return;

        let userFavourites = [];

        function normalise(text) {
            return (text || '')
                .replace(/\s+/g, ' ')
                .replace(/[^a-zA-Z0-9 &]+/g, '')
                .trim()
                .toUpperCase();
        }

        function normalizeFavouriteUrl(url) {
            if (!url) return '';

            try {
                const parsed = new URL(
                    url,
                    window.location.origin
                );

                // IMPORTANT:
                // formKey is intentionally ignored.
                // It can change when the same form is opened.
                return normalise(parsed.pathname);
            }
            catch (e) {
                return normalise(
                    url.split('?')[0]
                );
            }
        }

        function isDashboardChild(el) {
            return !!el.closest('.mis-dashboard-submenu') &&
                el.classList.contains('mis-dashboard-child');
        }

        function isNestedLeaf(li) {
            if (!li) return false;

            const parentSubmenu =
                li.closest('.dropdown-menu, .submenu');

            if (!parentSubmenu) return false;

            const nestedMenu =
                li.querySelector(
                    ':scope > .submenu, :scope > .dropdown-menu'
                );

            return !nestedMenu;
        }

        function isFavouriteTarget(li) {
            if (!li) return false;

            if (
                li.classList.contains(
                    'xon-favourites-item'
                )
            ) {
                return false;
            }

            // Dashboard tabs
            if (isDashboardChild(li)) {
                return true;
            }

            // Actual clickable leaf menu
            if (isNestedLeaf(li)) {
                return true;
            }

            return false;
        }

        function getTargetElement(li) {
            if (isDashboardChild(li)) {
                return li;
            }

            return li.querySelector(
                ':scope > .nav-link, :scope > a, :scope > button'
            );
        }

        /*
         * Returns a stable key for a menu.
         *
         * DO NOT use formKey as the primary identity because
         * formKey can change for the same form.
         */
        function favouriteKey(li) {

            const target =
                getTargetElement(li);

            if (!target) return '';

            // Dashboard child
            if (isDashboardChild(li)) {

                const dashboardKey =
                    target.getAttribute(
                        'data-dashboard-tab'
                    );

                if (dashboardKey) {
                    return normalise(
                        dashboardKey
                    );
                }
            }

            // Explicit permanent menu key
            const menuKey =
                target.getAttribute(
                    'data-menu-key'
                );

            if (menuKey) {
                return normalise(menuKey);
            }

            // URL pathname is the stable identity.
            // Query-string formKey is ignored.
            const href =
                target.getAttribute('href') || '';

            if (href) {
                const urlKey =
                    normalizeFavouriteUrl(href);

                if (urlKey) {
                    return urlKey;
                }
            }

            // Only use form-key attributes as a last fallback.
            const formKey =
                target.getAttribute('data-form-key') ||
                target.getAttribute('data-formkey');

            if (formKey) {
                return normalise(formKey);
            }

            // Last fallback
            const label =
                target.querySelector(
                    '.mis-label'
                )?.textContent ||
                target.getAttribute(
                    'data-dashboard-tab'
                ) ||
                target.textContent ||
                '';

            return normalise(label);
        }

        function getItemKey(item) {
            return (
                item?.MenuKey ||
                item?.menuKey ||
                ''
            ).toString().trim();
        }

        function getItemUrl(item) {
            return (
                item?.MenuUrl ||
                item?.menuUrl ||
                ''
            ).toString().trim();
        }

        /*
         * Find the actual DB favourite record for a menu.
         *
         * Matching order:
         * 1. MenuKey exact/stable match
         * 2. MenuUrl pathname match, ignoring formKey
         */
        function getFavouriteRecord(li) {

            if (!li) return null;

            const target =
                getTargetElement(li);

            if (!target) return null;

            const currentKey =
                normalise(
                    favouriteKey(li)
                );

            const currentUrl =
                normalizeFavouriteUrl(
                    target.getAttribute('href') || ''
                );

            const record =
                userFavourites.find(function (item) {

                    const dbKey =
                        normalise(
                            getItemKey(item)
                        );

                    // 1. Exact/stable MenuKey match
                    if (
                        currentKey &&
                        dbKey &&
                        currentKey === dbKey
                    ) {
                        return true;
                    }

                    // 2. MenuUrl pathname match
                    // Ignore ?formKey=...
                    const dbUrl =
                        normalizeFavouriteUrl(
                            getItemUrl(item)
                        );

                    if (
                        currentUrl &&
                        dbUrl &&
                        currentUrl === dbUrl
                    ) {
                        return true;
                    }

                    return false;
                });

            return record || null;
        }

        function isFavourite(liOrKey) {

            if (!liOrKey) {
                return false;
            }

            // Backward compatibility for any existing
            // code which still passes a string key.
            if (typeof liOrKey === 'string') {

                const key =
                    normalise(liOrKey);

                return userFavourites.some(
                    function (item) {
                        return normalise(
                            getItemKey(item)
                        ) === key;
                    }
                );
            }

            return !!getFavouriteRecord(
                liOrKey
            );
        }

        function setStarState(star, active) {
            if (!star) return;

            star.classList.toggle(
                'is-favourite',
                active
            );

            star.setAttribute(
                'aria-label',
                active
                    ? 'Remove from Favourites'
                    : 'Add to Favourites'
            );

            star.title =
                active
                    ? 'Remove from Favourites'
                    : 'Add to Favourites';

            star.innerHTML =
                active
                    ? '<i class="fas fa-star"></i>'
                    : '<i class="far fa-star"></i>';
        }

        function getAllFavouriteTargets() {

            const result = [];

            // Dashboard children
            nav.querySelectorAll(
                '.mis-dashboard-submenu .mis-dashboard-child'
            ).forEach(function (child) {

                if (isFavouriteTarget(child)) {
                    result.push(child);
                }
            });

            // Normal dropdown menu items
            nav.querySelectorAll(
                '.dropdown-menu li'
            ).forEach(function (li) {

                if (isFavouriteTarget(li)) {
                    result.push(li);
                }
            });

            // Nested submenu items
            nav.querySelectorAll(
                '.submenu li'
            ).forEach(function (li) {

                if (isFavouriteTarget(li)) {
                    result.push(li);
                }
            });

            return Array.from(
                new Set(result)
            );
        }

        async function loadUserFavourites() {

            const formKey =
                getCurrentFormKey();

            if (!formKey) {
                console.error(
                    'FormKey not found while loading favourites.'
                );
                return;
            }

            try {

                const response =
                    await fetch(
                        '/UserFavouriteMenu/GetUserFavouriteMenus?formKey=' +
                        encodeURIComponent(formKey),
                        {
                            method: 'GET',
                            credentials: 'include'
                        }
                    );

                if (!response.ok) {

                    console.error(
                        'Failed to load user favourites.',
                        response.status
                    );

                    userFavourites = [];

                    refreshFavouriteSection();

                    return;
                }

                const result =
                    await response.json();

                let data =
                    result?.data ??
                    result?.Data ??
                    result?.result ??
                    result?.Result ??
                    result;

                if (
                    data &&
                    !Array.isArray(data) &&
                    Array.isArray(data.data)
                ) {
                    data = data.data;
                }

                if (
                    data &&
                    !Array.isArray(data) &&
                    Array.isArray(data.Data)
                ) {
                    data = data.Data;
                }

                userFavourites =
                    Array.isArray(data)
                        ? data
                        : [];

                // Update stars AFTER DB data is loaded.
                getAllFavouriteTargets()
                    .forEach(function (li) {

                        const target =
                            getTargetElement(li);

                        if (!target) return;

                        const star =
                            target.querySelector(
                                ':scope > .xon-favourite-star'
                            );

                        setStarState(
                            star,
                            isFavourite(li)
                        );
                    });

                refreshFavouriteSection();

            }
            catch (error) {

                console.error(
                    'Error loading user favourites:',
                    error
                );

                userFavourites = [];

                refreshFavouriteSection();
            }
        }

        function addStar(li) {

            if (!isFavouriteTarget(li)) {
                return;
            }

            const target =
                getTargetElement(li);

            if (!target) return;

            let star =
                target.querySelector(
                    ':scope > .xon-favourite-star'
                );

            if (!star) {

                star =
                    document.createElement('span');

                star.className =
                    'xon-favourite-star';

                star.setAttribute(
                    'role',
                    'button'
                );

                star.setAttribute(
                    'tabindex',
                    '0'
                );

                target.appendChild(star);
            }

            setStarState(
                star,
                isFavourite(li)
            );

            if (
                star.dataset.xonFavouriteBound ===
                'true'
            ) {
                return;
            }

            star.dataset.xonFavouriteBound =
                'true';

            function handleFavouriteClick(e) {

                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();

                toggleFavourite(li);

                return false;
            }

            star.addEventListener(
                'click',
                handleFavouriteClick
            );

            star.addEventListener(
                'keydown',
                function (e) {

                    if (
                        e.key === 'Enter' ||
                        e.key === ' '
                    ) {
                        handleFavouriteClick(e);
                    }
                }
            );
        }

        async function toggleFavourite(li) {

            const target =
                getTargetElement(li);

            if (!target) {
                return;
            }

            const key =
                favouriteKey(li);

            const formKey =
                getCurrentFormKey();

            if (!key || !formKey) {
                console.error(
                    'Favourite key/formKey missing.',
                    {
                        key: key,
                        formKey: formKey
                    }
                );
                return;
            }

            const star =
                target.querySelector(
                    ':scope > .xon-favourite-star'
                );

            if (!star) {
                return;
            }

            const favouriteRecord =
                getFavouriteRecord(li);

            const alreadyFavourite =
                !!favouriteRecord;

            let localRecord = null;

            try {

                /* =====================================================
                   REMOVE FAVOURITE
                   ===================================================== */

                if (alreadyFavourite) {

                    const deleteMenuKey =
                        getItemKey(
                            favouriteRecord
                        ) || key;

                    const deleteFormKey =
                        favouriteRecord?.FormKey ||
                        favouriteRecord?.formKey ||
                        formKey;

                    const response =
                        await fetch(
                            '/UserFavouriteMenu/DeleteUserFavouriteMenu' +
                            '?MenuKey=' +
                            encodeURIComponent(
                                deleteMenuKey
                            ) +
                            '&formKey=' +
                            encodeURIComponent(
                                deleteFormKey
                            ),
                            {
                                method: 'DELETE',
                                credentials: 'include',
                                headers: {
                                    'Accept':
                                        'application/json'
                                }
                            }
                        );

                    if (!response.ok) {

                        const errorText =
                            await response.text();

                        console.error(
                            'Delete Favourite Failed:',
                            response.status,
                            errorText
                        );

                        return;
                    }

                    // Update local state immediately.
                    removeFavouriteFromLocalState(
                        favouriteRecord
                    );

                    // Immediately make star outline.
                    setStarState(
                        star,
                        false
                    );

                    // Immediately remove from MY FAVOURITES.
                    refreshFavouriteSection();

                    return;
                }


                /* =====================================================
                   ADD FAVOURITE
                   ===================================================== */

                const menuName =
                    target.querySelector(
                        '.mis-label'
                    )?.textContent?.trim() ||

                    target.getAttribute(
                        'data-dashboard-tab'
                    ) ||

                    target.textContent
                        ?.replace(/\s+/g, ' ')
                        .trim() ||

                    '';

                const originalMenuUrl =
                    target.getAttribute('href') || '';

                let menuUrl = originalMenuUrl;

                if (menuUrl && formKey) {

                    const url = new URL(
                        menuUrl,
                        window.location.origin
                    );

                    // Existing formKey હોય તો remove કરો
                    url.searchParams.delete('formKey');

                    // Current formKey add કરો
                    url.searchParams.set(
                        'formKey',
                        formKey
                    );

                    // DB માં save કરવા માટે relative URL
                    menuUrl =
                        url.pathname +
                        url.search +
                        url.hash;
                }
                /*
                 * IMPORTANT:
                 * Create the local record BEFORE calling the API.
                 *
                 * This makes the UI update immediately after the
                 * user clicks the star. It does not wait for the API
                 * response or a page refresh.
                 */
                localRecord = {

                    MenuKey:
                        key,

                    MenuName:
                        menuName,

                    MenuUrl:
                        menuUrl,

                    FormKey:
                        formKey

                };


                const exists =
                    userFavourites.some(
                        function (item) {

                            return (
                                normalise(
                                    getItemKey(item)
                                ) ===
                                normalise(
                                    localRecord.MenuKey
                                )
                            );

                        }
                    );


                if (!exists) {

                    userFavourites.push(
                        localRecord
                    );

                }


                // =====================================================
                // IMMEDIATE UI UPDATE
                // =====================================================

                setStarState(
                    star,
                    true
                );

                refreshFavouriteSection();


                // =====================================================
                // SAVE TO DATABASE
                // =====================================================

                const response =
                    await fetch(
                        '/UserFavouriteMenu/AddUserFavouriteMenu' +
                        '?formKey=' +
                        encodeURIComponent(
                            formKey
                        ),
                        {
                            method: 'POST',
                            credentials: 'include',

                            headers: {
                                'Content-Type':
                                    'application/json',

                                'Accept':
                                    'application/json'
                            },

                            body: JSON.stringify({

                                FormKey:
                                    formKey,

                                MenuKey:
                                    key,

                                MenuName:
                                    menuName,

                                MenuUrl:
                                    menuUrl

                            })
                        }
                    );


                /*
                 * Do NOT depend on response JSON.
                 *
                 * Your backend can successfully save the record but
                 * still return a serialization error because the BLL
                 * result may contain DataTable/System.Type/etc.
                 *
                 * The UI has already been updated above.
                 */

                if (!response.ok) {

                    const errorText =
                        await response.text();

                    console.error(
                        'Add Favourite API returned:',
                        response.status,
                        errorText
                    );

                    /*
                     * Sync from DB.
                     *
                     * If DB insert succeeded but response
                     * serialization failed, GET will restore the
                     * favourite correctly.
                     */
                    setTimeout(
                        function () {
                            loadUserFavourites();
                        },
                        0
                    );

                    return;
                }


                /*
                 * API success.
                 *
                 * Keep the local UI as-is and perform a background
                 * GET only to synchronize the exact DB record.
                 *
                 * No refresh is required.
                 */
                loadUserFavourites()
                    .catch(
                        function (error) {

                            console.warn(
                                'Favourite background sync failed:',
                                error
                            );

                        }
                    );

            }
            catch (error) {

                console.error(
                    'Favourite operation failed:',
                    error
                );

                /*
                 * If POST itself failed before reaching the server,
                 * restore the previous state.
                 *
                 * Do NOT immediately clear the UI for a response
                 * serialization error; loadUserFavourites() above
                 * handles that case.
                 */
                if (!alreadyFavourite) {

                    userFavourites =
                        userFavourites.filter(
                            function (item) {
                                return item !==
                                    localRecord;
                            }
                        );

                    setStarState(
                        star,
                        false
                    );

                    refreshFavouriteSection();
                }
            }
        }

        function createFavouriteSection() {

            let container =
                nav.querySelector(
                    ':scope > .xon-favourites-item'
                );

            if (container) {
                return container;
            }

            container = document.createElement('li');

            container.className =
                'nav-item xon-favourites-item is-collapsed';

            container.innerHTML = `
                <div class="xon-favourites-header"
                     role="button"
                     tabindex="0"
                     aria-expanded="false">

                    <span class="xon-favourites-title">
                        <i class="fas fa-bookmark"></i>
                        <span>MY FAVOURITES</span>
                    </span>

                    <span class="xon-favourites-chevron">▾</span>
                </div>

                <div class="xon-favourites-list"
                     aria-live="polite"
                     style="display:none;">
                </div>
            `;

            const dashboardItem =
                nav.querySelector(':scope > .nav-item');

            if (dashboardItem) {
                dashboardItem.insertAdjacentElement(
                    'beforebegin',
                    container
                );
            } else {
                nav.prepend(container);
            }

            const header =
                container.querySelector(
                    '.xon-favourites-header'
                );

            const list =
                container.querySelector(
                    '.xon-favourites-list'
                );

            const chevron =
                container.querySelector(
                    '.xon-favourites-chevron'
                );

            if (!header || !list) {
                return container;
            }

            /*
             * =========================================================
             * MY FAVOURITES OPEN / CLOSE
             * =========================================================
             *
             * Keep the favourites section independent from the
             * Bootstrap/sidebar tree menu behaviour.
             */

            // Initial state = CLOSED
            container.classList.add('is-collapsed');

            list.style.setProperty(
                'display',
                'none',
                'important'
            );

            header.setAttribute(
                'aria-expanded',
                'false'
            );

            if (chevron) {
                chevron.textContent = '▾';
            }

            /*
             * =========================================================
             * IMPORTANT
             * =========================================================
             *
             * Do not bind the click directly to this header.
             * The ERP sidebar is rebuilt/modified by other scripts.
             *
             * The actual open/close handler is installed once at
             * document level below setupFavouriteMenus().
             *
             * This makes MY FAVOURITES work even if the sidebar/header
             * is recreated after this function runs.
             */

            return container;
        }


        function removeFavouriteFromLocalState(record) {

            if (!record) {
                return;
            }

            const recordKey =
                normalise(getItemKey(record));

            userFavourites =
                userFavourites.filter(function (item) {

                    return normalise(
                        getItemKey(item)
                    ) !== recordKey;

                });
        }

        function refreshFavouriteSection() {

            const container =
                createFavouriteSection();

            const list =
                container.querySelector(
                    '.xon-favourites-list'
                );

            if (!list) return;

            list.innerHTML = '';

            /*
             * IMPORTANT:
             * Build MY FAVOURITES DIRECTLY from userFavourites.
             *
             * Do NOT filter through getAllFavouriteTargets().
             * A favourite can exist in the database even when its
             * menu is nested/dynamically rendered in the DOM.
             *
             * This also makes the list update immediately after
             * Add/Delete without requiring a page refresh.
             */
            const selected =
                Array.isArray(userFavourites)
                    ? userFavourites.slice()
                    : [];

            if (!selected.length) {

                container.classList.add(
                    'is-empty'
                );

                return;
            }

            container.classList.remove(
                'is-empty'
            );

            selected.forEach(
                function (record) {

                    if (!record) return;

                    const menuKey =
                        getItemKey(record);

                    const menuName =
                        record?.MenuName ||
                        record?.menuName ||
                        'Unnamed Menu';

                    const menuUrl =
                        record?.MenuUrl ||
                        record?.menuUrl ||
                        '';

                    const row =
                        document.createElement(
                            'div'
                        );

                    row.className =
                        'xon-favourite-row';

                    row.innerHTML = `
                        <button type="button"
                                class="xon-favourite-open"
                                title="Open">

                            <span class="xon-favourite-row-icon">
                                <i class="fas fa-star"></i>
                            </span>

                            <span class="xon-favourite-row-text">
                            </span>

                        </button>

                        <button type="button"
                                class="xon-favourite-remove"
                                aria-label="Remove from Favourites"
                                title="Remove from Favourites">

                            <i class="fas fa-xmark"></i>

                        </button>
                    `;

                    row.querySelector(
                        '.xon-favourite-row-text'
                    ).textContent =
                        menuName.toString().trim();


                    /*
                     * OPEN FAVOURITE
                     *
                     * Prefer the saved MenuUrl.
                     * This does not depend on the current formKey.
                     */
                    row.querySelector(
                        '.xon-favourite-open'
                    ).addEventListener(
                        'click',
                        function (e) {

                            e.preventDefault();
                            e.stopPropagation();

                            if (menuUrl) {

                                let finalUrl = menuUrl;

                                const currentFormKey =
                                    getCurrentFormKey();

                                if (currentFormKey) {

                                    const url = new URL(
                                        menuUrl,
                                        window.location.origin
                                    );

                                    // Remove old formKey if exists
                                    url.searchParams.delete('formKey');

                                    // Add current formKey
                                    url.searchParams.set(
                                        'formKey',
                                        currentFormKey
                                    );

                                    finalUrl =
                                        url.pathname +
                                        url.search +
                                        url.hash;
                                }

                                console.log(
                                    'Favourite Menu URL:',
                                    finalUrl
                                );

                                window.location.href =
                                    finalUrl;

                                return;
                            }

                            /*
                             * Fallback for dashboard favourites.
                             */
                            const dashboardTarget =
                                nav.querySelector(
                                    '.mis-dashboard-child[data-dashboard-tab="' +
                                    CSS.escape(menuName) +
                                    '"]'
                                );

                            if (dashboardTarget) {

                                dashboardTarget.click();

                            }

                        }
                    );


                    /*
                     * REMOVE FAVOURITE
                     */
                    row.querySelector(
                        '.xon-favourite-remove'
                    ).addEventListener(
                        'click',
                        async function (e) {

                            e.preventDefault();
                            e.stopPropagation();
                            e.stopImmediatePropagation();

                            if (!menuKey) {

                                console.error(
                                    'Favourite MenuKey missing.',
                                    record
                                );

                                return;
                            }

                            /*
                             * Use the FormKey stored with the
                             * favourite record first.
                             */
                            const deleteFormKey =
                                (
                                    record?.FormKey ||
                                    record?.formKey ||
                                    getCurrentFormKey()
                                )
                                    .toString()
                                    .trim();

                            if (!deleteFormKey) {

                                console.error(
                                    'FormKey not found while removing favourite.'
                                );

                                return;
                            }

                            try {

                                const response =
                                    await fetch(
                                        '/UserFavouriteMenu/DeleteUserFavouriteMenu' +
                                        '?MenuKey=' +
                                        encodeURIComponent(
                                            menuKey
                                        ) +
                                        '&formKey=' +
                                        encodeURIComponent(
                                            deleteFormKey
                                        ),
                                        {
                                            method:
                                                'DELETE',

                                            credentials:
                                                'include',

                                            headers: {
                                                'Accept':
                                                    'application/json'
                                            }
                                        }
                                    );

                                if (!response.ok) {

                                    const errorText =
                                        await response.text();

                                    console.error(
                                        'Failed to remove favourite:',
                                        response.status,
                                        errorText
                                    );

                                    return;
                                }


                                /*
                                 * IMMEDIATE LOCAL UPDATE
                                 */
                                userFavourites =
                                    userFavourites.filter(
                                        function (item) {

                                            return normalise(getItemKey(item)) !==
                                                normalise(getItemKey(record));

                                        }
                                    );


                                /*
                                 * IMMEDIATE LIST UPDATE
                                 *
                                 * Remove by MenuKey, not object reference.
                                 */
                                row.remove();

                                /*
                                 * Rebuild the complete list from the
                                 * updated local array. This guarantees
                                 * that the deleted favourite cannot
                                 * remain visible.
                                 */
                                refreshFavouriteSection();


                                /*
                                 * IMMEDIATELY UPDATE ORIGINAL
                                 * MENU STAR, if that menu is
                                 * currently present in DOM.
                                 */
                                getAllFavouriteTargets()
                                    .forEach(
                                        function (li) {

                                            const currentRecord =
                                                getFavouriteRecord(
                                                    li
                                                );

                                            /*
                                             * currentRecord will now
                                             * be null for the deleted
                                             * favourite.
                                             */
                                            const currentKey =
                                                favouriteKey(
                                                    li
                                                );

                                            if (
                                                normalise(
                                                    currentKey
                                                ) ===
                                                normalise(
                                                    menuKey
                                                )
                                            ) {

                                                const target =
                                                    getTargetElement(
                                                        li
                                                    );

                                                const star =
                                                    target?.querySelector(
                                                        ':scope > .xon-favourite-star'
                                                    );

                                                setStarState(
                                                    star,
                                                    false
                                                );
                                            }

                                        }
                                    );


                                // Always re-bind the list after deletion.
                                refreshFavouriteSection();

                            }
                            catch (error) {

                                console.error(
                                    'Failed to remove favourite:',
                                    error
                                );

                            }

                        }
                    );


                    list.appendChild(row);

                }
            );
        }

        /*
         * Remove incorrectly generated stars from
         * top-level parent menu items.
         */
        nav.querySelectorAll(
            ':scope > .navbar-nav > .nav-item > .nav-link > .xon-favourite-star'
        ).forEach(
            function (star) {
                star.remove();
            }
        );

        /*
         * Add stars ONLY to actual favourite targets.
         */
        getAllFavouriteTargets()
            .forEach(addStar);

        /*
         * Create MY FAVOURITES section.
         */
        refreshFavouriteSection();

        /*
         * Load favourites from database and then
         * update the stars.
         */
        loadUserFavourites();
    }

    function installFavouriteToggleStyle() {

        if (
            document.getElementById(
                "xon-favourite-toggle-final-style"
            )
        ) {
            return;
        }

        const style =
            document.createElement("style");

        style.id =
            "xon-favourite-toggle-final-style";

        style.textContent = `
            #main_nav .xon-favourites-item {
                position: relative !important;
                width: 100% !important;
            }

            #main_nav .xon-favourites-header {
                display: flex !important;
                align-items: center !important;
                justify-content: space-between !important;
                width: 100% !important;
                min-height: 38px !important;
                box-sizing: border-box !important;
                cursor: pointer !important;
                user-select: none !important;
                pointer-events: auto !important;
                position: relative !important;
                z-index: 9999 !important;
            }

            #main_nav .xon-favourites-header * {
                pointer-events: none !important;
            }

            #main_nav .xon-favourites-list {
                width: 100% !important;
                box-sizing: border-box !important;
                position: relative !important;
                z-index: 9998 !important;
            }

            #main_nav .xon-favourites-item.is-collapsed
                > .xon-favourites-list {
                display: none !important;
            }

            #main_nav .xon-favourites-item:not(.is-collapsed)
                > .xon-favourites-list {
                display: block !important;
                visibility: visible !important;
                opacity: 1 !important;
                max-height: 10000px !important;
            }
        `;

        document.head.appendChild(style);
    }

    function setupMISNav() {
        installFavouriteToggle();


        ensureDashboardMenuStyles();

        const nav = document.querySelector('#main_nav>.navbar-nav');

        if (!nav) return;

        const items = Array.from(nav.children)
            .filter(x => x.classList.contains('nav-item'));

        /*
         * IMPORTANT:
         * MY FAVOURITES can be inserted before Dashboard.
         * Never assume Dashboard is dashboardItemRef.
         */
        const dashboardItemRef =
            items.find(function (item) {
                const link =
                    item.querySelector(':scope>.nav-link');

                return (
                    link &&
                    (link.textContent || '')
                        .trim()
                        .replace(/\s+/g, ' ')
                        .toUpperCase() === 'DASHBOARD'
                );
            }) || null;

        /* =========================================================
           MENU LABELS
           ========================================================= */

        items.forEach((li, i) => {

            const a = li.querySelector(':scope>.nav-link');

            if (!a) return;

            const originalText = (a.textContent || '')
                .trim()
                .replace(/\s+/g, ' ')
                .toUpperCase();

            const def = labels[originalText];

            if (!def) return;

            applyMenu(a, def);
        });

        /* =========================================================
           ACTIVE MENU
           Only ONE top-level menu is active at a time.
           Dashboard is NOT forced active by the URL after another
           top-level menu has been selected.
           ========================================================= */

        function getMenuKey(item) {
            if (!item) return '';

            const link = item.querySelector(':scope>.nav-link');

            if (!link) return '';

            return (link.textContent || '')
                .trim()
                .replace(/\s+/g, ' ')
                .toUpperCase();
        }

        function clearActiveMenu() {

            items.forEach(function (item) {

                const link =
                    item.querySelector(':scope>.nav-link');

                if (link) {
                    link.classList.remove(
                        'mis-active',
                        'active',
                        'xon-parent-active',
                        'xon-child-active',
                        'xon-child-child-active'
                    );
                }

                /*
                 * IMPORTANT:
                 * Do NOT remove mis-dashboard-expanded here.
                 *
                 * Dashboard click does:
                 *   1. expand Dashboard
                 *   2. setActiveMenu()
                 *
                 * Removing the expanded class here was hiding
                 * Inventory/Purchase/Sales/Production/MIS.
                 */
                item.classList.remove('active');

                item.querySelectorAll('a.active').forEach(
                    function (childLink) {

                        childLink.classList.remove(
                            'active',
                            'xon-parent-active',
                            'xon-child-active',
                            'xon-child-child-active'
                        );
                    }
                );
            });
        }

        function setActiveMenu(activeItem, remember = true) {

            clearActiveMenu();

            if (!activeItem) return;

            const activeLink =
                activeItem.querySelector(':scope>.nav-link');

            if (activeLink) {
                activeLink.classList.add(
                    'mis-active',
                    'active',
                    'xon-parent-active'
                );
            }

            activeItem.classList.add('active');

            if (remember) {
                window.__xonActiveTopMenu =
                    getMenuKey(activeItem);
            }
        }

        function getInitialActiveMenu() {

            const rememberedKey =
                window.__xonActiveTopMenu;

            if (rememberedKey) {
                const remembered =
                    items.find(function (item) {
                        return getMenuKey(item) === rememberedKey;
                    });

                if (remembered) {
                    return remembered;
                }
            }

            const path = location.pathname.toLowerCase();

            let activeItem = null;

            items.forEach(function (li, i) {

                const a =
                    li.querySelector(':scope>.nav-link');

                if (!a || activeItem) return;

                let active = false;

                /*
                 * Dashboard is active on the actual Dashboard page
                 * only when no previous top-level selection exists.
                 */
                if (
                    li === dashboardItemRef &&
                    (
                        path === '/' ||
                        path === '/home' ||
                        path === '/home/dashboard'
                    )
                ) {
                    active = true;
                }

                /*
                 * Normal menu pages are detected from their child URLs.
                 */
                li.querySelectorAll('a[href]').forEach(function (link) {

                    const href =
                        (link.getAttribute('href') || '')
                            .toLowerCase();

                    if (
                        href &&
                        href !== '#' &&
                        href !== 'javascript:void(0)' &&
                        path !== '/' &&
                        href.includes(path)
                    ) {
                        active = true;
                    }
                });

                if (active) {
                    activeItem = li;
                }
            });

            return activeItem;
        }

        const initialActiveItem =
            getInitialActiveMenu();

        setActiveMenu(
            initialActiveItem,
            !!initialActiveItem
        );

        /* =========================================================
           DASHBOARD PARENT MENU

           Dashboard
              ├── Inventory
              ├── Purchase
              ├── Sales
              ├── Production
              └── MIS
           ========================================================= */

        function setupDashboardParentMenu() {

            const dashboardItem = dashboardItemRef;

            if (!dashboardItem) return;

            const dashboardLink =
                dashboardItem.querySelector(':scope>.nav-link');

            if (!dashboardLink) return;

            let dashboardSubmenu =
                dashboardItem.querySelector(':scope>.mis-dashboard-submenu');

            if (!dashboardSubmenu) {

                dashboardSubmenu = document.createElement('div');

                dashboardSubmenu.className =
                    'mis-dashboard-submenu';

                dashboardSubmenu.setAttribute(
                    'aria-hidden',
                    'true'
                );

                dashboardSubmenu.innerHTML = `
                    <button type="button"
                            class="mis-dashboard-child"
                            data-dashboard-tab="Inventory Dashboard">
                        <i class="fas fa-boxes-stacked"></i>
                        <span>Inventory</span>
                    </button>

                    <button type="button"
                            class="mis-dashboard-child"
                            data-dashboard-tab="Purchase Dashboard">
                        <i class="fas fa-file-invoice"></i>
                        <span>Purchase</span>
                    </button>

                    <button type="button"
                            class="mis-dashboard-child"
                            data-dashboard-tab="Sales Dashboard">
                        <i class="fas fa-file-invoice-dollar"></i>
                        <span>Sales</span>
                    </button>

                    <button type="button"
                            class="mis-dashboard-child"
                            data-dashboard-tab="Production Dashboard">
                        <i class="fas fa-industry"></i>
                        <span>Production</span>
                    </button>

                    <button type="button"
                            class="mis-dashboard-child"
                            data-dashboard-tab="MIS Dashboard">
                        <i class="fas fa-chart-line"></i>
                        <span>MIS</span>
                    </button>
                `;

                dashboardItem.appendChild(dashboardSubmenu);
            }

            dashboardLink.classList.add('mis-dashboard-parent');

            /*
             * Dashboard click:
             * - On another page: navigate to Dashboard.
             * - Already on Dashboard: ONLY expand/collapse.
             * - Never trigger another click handler/data refresh.
             */

            dashboardLink.addEventListener('click', function (e) {

                e.preventDefault();

                /*
                 * IMPORTANT:
                 * Dashboard parent click must NOT trigger any other
                 * delegated/Bootstrap handler which can reload or
                 * refresh dashboard data.
                 *
                 * We handle Dashboard expand/collapse ourselves below.
                 */
                e.stopImmediatePropagation();

                const currentPath =
                    (window.location.pathname || '/').toLowerCase();

                const isDashboardPage =
                    currentPath === '/' ||
                    currentPath === '/home' ||
                    currentPath === '/home/' ||
                    currentPath === '/home/dashboard' ||
                    currentPath === '/home/dashboard/';

                /*
                 * If the user is on another page, Dashboard should
                 * still navigate to the Dashboard page.
                 */
                if (!isDashboardPage) {
                    const dashboardUrl =
                        dashboardLink.getAttribute('href');

                    if (
                        dashboardUrl &&
                        dashboardUrl !== '#' &&
                        dashboardUrl !== 'javascript:void(0)'
                    ) {
                        window.location.href = dashboardUrl;
                    } else {
                        window.location.href = '/Home/Dashboard';
                    }

                    return;
                }

                /*
                 * Already on Dashboard: do NOT reload the page.
                 * Simply expand/collapse the Dashboard children.
                 */
                const isExpanded =
                    dashboardItem.classList.contains('mis-dashboard-expanded');

                setActiveMenu(dashboardItem, true);

                if (isExpanded) {
                    dashboardItem.classList.remove('mis-dashboard-expanded');
                    dashboardSubmenu.setAttribute('aria-hidden', 'true');
                } else {
                    dashboardItem.classList.add('mis-dashboard-expanded');
                    dashboardSubmenu.setAttribute('aria-hidden', 'false');
                }
            });

            const childButtons =
                dashboardSubmenu.querySelectorAll(
                    '.mis-dashboard-child'
                );

            /* Child click */

            childButtons.forEach(function (child) {

                child.addEventListener('click', function (e) {

                    e.preventDefault();
                    e.stopPropagation();

                    /* A real user selection must not be overwritten by
                       the delayed initial dashboard resolver. */
                    window.__xonDashboardUserInteracted = true;

                    const tabName =
                        child.getAttribute('data-dashboard-tab');

                    setActiveMenu(dashboardItem, true);

                    dashboardItem.classList.add(
                        'mis-dashboard-expanded'
                    );

                    dashboardSubmenu.setAttribute(
                        'aria-hidden',
                        'false'
                    );

                    childButtons.forEach(function (x) {
                        x.classList.remove('active');
                    });

                    child.classList.add('active');

                    /*
                     * Dashboard breadcrumb:
                     * Dashboard > Inventory Dashboard
                     */
                    if (
                        typeof window.xonUpdateBreadcrumb ===
                        'function'
                    ) {
                        window.xonUpdateBreadcrumb();
                    }

                    /*
                     * Click the existing dashboard tab.
                     * No new route is created.
                     */

                    /*
                     * IMPORTANT:
                     * Never search inside #main_nav here. The dashboard
                     * child buttons are inside the sidebar and must NOT
                     * be clicked recursively as if they were real tabs.
                     */
                    const target =
                        getDashboardTopTabByName(tabName);

                    if (target) {
                        target.click();
                    }
                });
            });

            /*
             * Existing top dashboard tabs -> sync left submenu.
             */

            const possibleTabs =
                document.querySelectorAll(
                    'button, a, [role="tab"], .nav-link, .dashboard-tab'
                );

            Array.from(possibleTabs).forEach(function (tab) {

                const tabText =
                    (tab.textContent || '')
                        .trim()
                        .replace(/\s+/g, ' ')
                        .toLowerCase();

                const matchingChild =
                    Array.from(childButtons).find(function (child) {

                        return (
                            child.getAttribute('data-dashboard-tab')
                                .toLowerCase() === tabText
                        );
                    });

                if (!matchingChild) return;

                tab.addEventListener('click', function () {

                    /*
                     * If this tab click was triggered automatically
                     * during Dashboard page initialization, keep the
                     * left Dashboard menu COLLAPSED.
                     *
                     * A real user click on a dashboard tab should
                     * still open Dashboard and select that child.
                     */
                    const autoSelecting =
                        window.__xonDashboardAutoSelecting === true;

                    if (autoSelecting) {
                        dashboardItem.classList.remove(
                            'mis-dashboard-expanded'
                        );

                        dashboardSubmenu.setAttribute(
                            'aria-hidden',
                            'true'
                        );
                    } else {
                        dashboardItem.classList.add(
                            'mis-dashboard-expanded'
                        );

                        dashboardSubmenu.setAttribute(
                            'aria-hidden',
                            'false'
                        );
                    }

                    setActiveMenu(dashboardItem, true);

                    childButtons.forEach(function (x) {
                        x.classList.remove('active');
                    });

                    matchingChild.classList.add('active');

                    /*
                     * Sync breadcrumb after the dashboard tab
                     * has become active.
                     */
                    if (
                        typeof window.xonUpdateBreadcrumb ===
                        'function'
                    ) {
                        window.xonUpdateBreadcrumb();
                    }
                });
            });
        }

        /* =========================================================
           XON ERP - DASHBOARD INITIAL SELECTION

           Priority:
             1. URL hash
             2. Existing active sidebar dashboard child
             3. Existing active dashboard tab
             4. Inventory Dashboard fallback

           Examples:
             #inventoryTab  -> Inventory Dashboard
             #purchaseTab   -> Purchase Dashboard
             #salesTab      -> Sales Dashboard
             #productionTab -> Production Dashboard
             #misTab        -> MIS Dashboard

           IMPORTANT:
           The Dashboard submenu remains collapsed on initial load.
           ========================================================= */

        function initializeDashboardSelection() {

            /* Never overwrite a real user action. */
            if (window.__xonDashboardUserInteracted === true) {
                return;
            }

            const dashboardItem = dashboardItemRef;

            if (!dashboardItem) {
                return;
            }

            const dashboardSubmenu =
                dashboardItem.querySelector(
                    ':scope>.mis-dashboard-submenu'
                );

            if (!dashboardSubmenu) {
                return;
            }

            const currentPath =
                (window.location.pathname || '/')
                    .toLowerCase()
                    .replace(/\/+$/, '');

            const isDashboardPage =
                currentPath === '' ||
                currentPath === '/' ||
                currentPath === '/home' ||
                currentPath === '/home/dashboard';

            if (!isDashboardPage) {
                return;
            }

            /* 1. URL hash has highest priority. */
            let dashboardName =
                getDashboardNameFromHash();

            /* 2. Existing active sidebar child. */
            if (!dashboardName) {

                const activeChild =
                    dashboardSubmenu.querySelector(
                        '.mis-dashboard-child.active'
                    );

                if (activeChild) {
                    dashboardName =
                        activeChild.getAttribute(
                            'data-dashboard-tab'
                        ) || '';
                }
            }

            /* 3. Existing active real dashboard tab. */
            if (!dashboardName) {

                const dashboardNames = [
                    'Inventory Dashboard',
                    'Purchase Dashboard',
                    'Sales Dashboard',
                    'Production Dashboard',
                    'MIS Dashboard'
                ];

                const activeTab =
                    Array.from(
                        document.querySelectorAll(
                            'button, a, [role="tab"], .nav-link, .dashboard-tab'
                        )
                    ).find(function (tab) {

                        if (
                            tab.classList.contains(
                                'mis-dashboard-child'
                            )
                        ) {
                            return false;
                        }

                        if (tab.closest('#main_nav')) {
                            return false;
                        }

                        if (!tab.classList.contains('active')) {
                            return false;
                        }

                        const text =
                            (tab.textContent || '')
                                .replace(/\s+/g, ' ')
                                .trim()
                                .toLowerCase();

                        return dashboardNames.some(function (name) {
                            return name.toLowerCase() === text;
                        });
                    });

                if (activeTab) {
                    dashboardName =
                        (activeTab.textContent || '')
                            .replace(/\s+/g, ' ')
                            .trim();
                }
            }

            /* 4. Final fallback only when nothing else is available. */
            if (!dashboardName) {
                dashboardName = 'Inventory Dashboard';
            }

            const selectedChild =
                getDashboardChildByName(
                    dashboardSubmenu,
                    dashboardName
                );

            if (!selectedChild) {
                return;
            }

            /* Dashboard is active, but submenu remains collapsed. */
            setActiveMenu(
                dashboardItem,
                false
            );

            dashboardItem.classList.remove(
                'mis-dashboard-expanded'
            );

            dashboardSubmenu.setAttribute(
                'aria-hidden',
                'true'
            );

            /* Select exactly the dashboard represented by the URL. */
            dashboardSubmenu
                .querySelectorAll('.mis-dashboard-child')
                .forEach(function (child) {
                    child.classList.remove('active');
                });

            selectedChild.classList.add('active');

            /*
             * Mark the real top dashboard tab as active without clicking it.
             * This prevents an unnecessary refresh/data reload during startup.
             */
            const dashboardTabNames = [
                'Inventory Dashboard',
                'Purchase Dashboard',
                'Sales Dashboard',
                'Production Dashboard',
                'MIS Dashboard'
            ];

            document.querySelectorAll(
                'button, a, [role="tab"], .nav-link, .dashboard-tab'
            ).forEach(function (tab) {

                if (tab.classList.contains('mis-dashboard-child')) {
                    return;
                }

                if (tab.closest('#main_nav')) {
                    return;
                }

                const text = (tab.textContent || '')
                    .replace(/\s+/g, ' ')
                    .trim()
                    .toLowerCase();

                if (dashboardTabNames.some(function (name) {
                    return name.toLowerCase() === text;
                })) {
                    tab.classList.remove('active');
                }
            });

            const realDashboardTab =
                getDashboardTopTabByName(
                    dashboardName
                );

            if (realDashboardTab) {
                realDashboardTab.classList.add('active');
            }

            /* Breadcrumb follows the same URL/dashboard selection. */
            if (
                typeof window.xonUpdateBreadcrumb ===
                'function'
            ) {
                window.xonUpdateBreadcrumb();
            }
        }

        setupDashboardParentMenu();

        /* Give dashboard tabs time to render before resolving the URL hash. */
        window.setTimeout(function () {
            initializeDashboardSelection();
        }, 500);

        items.forEach(function (li) {

            const link =
                li.querySelector(':scope>.nav-link');

            if (!link) return;

            if (li.dataset.xonMenuBound === 'true') {
                return;
            }

            li.dataset.xonMenuBound = 'true';

            if (li !== dashboardItemRef) {

                link.addEventListener('click', function () {

                    setActiveMenu(li, true);

                    if (dashboardItemRef) {

                        const dashboardItem = dashboardItemRef;

                        dashboardItem.classList.remove(
                            'active',
                            'mis-dashboard-expanded'
                        );

                        const dashboardLink =
                            dashboardItem.querySelector(
                                ':scope>.nav-link'
                            );

                        if (dashboardLink) {
                            dashboardLink.classList.remove(
                                'mis-active'
                            );
                        }

                        const dashboardSubmenu =
                            dashboardItem.querySelector(
                                ':scope>.mis-dashboard-submenu'
                            );

                        if (dashboardSubmenu) {
                            dashboardSubmenu.setAttribute(
                                'aria-hidden',
                                'true'
                            );
                        }
                    }
                });
            }

            if (li.classList.contains('dropdown')) {

                li.addEventListener(
                    'show.bs.dropdown',
                    function () {
                        if (li !== dashboardItemRef) {
                            setActiveMenu(li, true);
                        }
                    }
                );

                li.addEventListener(
                    'shown.bs.dropdown',
                    function () {
                        if (li !== dashboardItemRef) {
                            setActiveMenu(li, true);
                        }
                    }
                );
            }

            li.querySelectorAll(
                '.dropdown-menu a[href]'
            ).forEach(function (childLink) {

                childLink.addEventListener(
                    'click',
                    function () {

                        setActiveMenu(li, true);

                        if (dashboardItemRef && li !== dashboardItemRef) {

                            dashboardItemRef.classList.remove(
                                'active',
                                'mis-dashboard-expanded'
                            );

                            const dashboardLink =
                                dashboardItemRef.querySelector(
                                    ':scope>.nav-link'
                                );

                            if (dashboardLink) {
                                dashboardLink.classList.remove(
                                    'mis-active'
                                );
                            }

                            const dashboardSubmenu =
                                dashboardItemRef.querySelector(
                                    ':scope>.mis-dashboard-submenu'
                                );

                            if (dashboardSubmenu) {
                                dashboardSubmenu.setAttribute(
                                    'aria-hidden',
                                    'true'
                                );
                            }
                        }
                    }
                );
            });
        });

        // Build the Favourite section after all top-level menu labels/stars exist.
        setupFavouriteMenus(nav);
    }

    if (document.readyState === 'loading') {
        document.addEventListener(
            'DOMContentLoaded',
            setupMISNav
        );
    } else {
        setupMISNav();
    }

})();

(function () {
    "use strict";

    /*
 * =========================================================
 * XON ERP - DATABASE ICON BINDING
 * ---------------------------------------------------------
 * IMPORTANT:
 * Menu icons are already supplied by MenuModel.IconClass
 * from the database/Razor view.
 *
 * DO NOT hard-code menu-name -> icon mappings here.
 *
 * Required visual order:
 *     [ +/- ] [ DATABASE MENU ICON ] [ MENU NAME ]
 * =========================================================
 */

    function addChildIcons(nav) {
        if (!nav) {
            return;
        }

        nav.querySelectorAll(
            "ul.dropdown-menu > li > a.dropdown-item, " +
            "ul.submenu > li > a.dropdown-item"
        ).forEach(function (link) {

            const treeArrow = link.querySelector(
                ":scope > .xon-tree-arrow"
            );

            /*
             * Keep the icon rendered by Razor from MenuModel.IconClass.
             * If an older script has injected an .xon-child-icon,
             * prefer the existing DB icon and remove the injected one.
             */
            const directIcons = Array.from(link.children).filter(function (el) {
                return el.tagName === "I" &&
                    !el.classList.contains("xon-tree-arrow") &&
                    !el.classList.contains("xon-favourite-star");
            });

            if (!directIcons.length) {
                /*
                 * No icon was supplied by the database.
                 * Do not invent one here.
                 */
                return;
            }

            const dbIcon =
                directIcons.find(function (el) {
                    return !el.classList.contains("xon-child-icon");
                }) || directIcons[0];

            /*
             * Remove only duplicate/injected menu icons.
             * The first real DB icon is retained.
             */
            directIcons.forEach(function (el) {
                if (el !== dbIcon) {
                    el.remove();
                }
            });

            /*
             * Marker class is only for CSS/layout.
             * It does NOT change the database icon class.
             */
            dbIcon.classList.add("xon-child-icon");
            dbIcon.setAttribute("aria-hidden", "true");

            /*
             * Force DOM order without changing the icon:
             * [ +/- ] -> [ DB icon ] -> [ label ]
             */
            if (treeArrow && treeArrow.nextElementSibling !== dbIcon) {
                treeArrow.insertAdjacentElement("afterend", dbIcon);
            }
        });
    }

    function normalizeMenuText(link) {
        const copy = link.cloneNode(true);

        copy.querySelectorAll(
            "i, .xon-tree-arrow, .xon-label, .mis-label"
        ).forEach(function (node) {
            node.remove();
        });

        return (copy.textContent || "")
            .replace(/\s+/g, " ")
            .trim()
            .toUpperCase();
    }

    function addChildIcons(nav) {
        nav.querySelectorAll(
            "ul.dropdown-menu > li > a.dropdown-item, " +
            "ul.submenu > li > a.dropdown-item"
        ).forEach(function (link) {

            /*
             * IMPORTANT:
             * Some menu items already contain their own <i> icon.
             * Do NOT add another icon. Reuse the existing icon and
             * keep exactly this order:
             * [ +/- ] [ MENU ICON ] [ MENU NAME ]
             */
            const treeArrow = link.querySelector(
                ":scope > .xon-tree-arrow"
            );

            const directIcons = Array.from(link.children).filter(function (el) {
                return el.tagName === "I" &&
                    !el.classList.contains("xon-tree-arrow") &&
                    !el.classList.contains("xon-favourite-star");
            });

            let icon = directIcons.find(function (el) {
                return el.classList.contains("xon-child-icon");
            }) || directIcons[0] || null;

            /* Remove duplicate direct icons. Keep only ONE menu icon. */
            directIcons.forEach(function (el) {
                if (el !== icon) {
                    el.remove();
                }
            });

            /*
             * IMPORTANT: Menu icons come ONLY from the database
             * (MenuModel.IconClass rendered by Razor).
             * Never create or replace an icon here.
             */
            if (!icon) {
                return;
            }

            /* Marker class is for layout only; preserve the DB icon class. */
            icon.classList.add("xon-child-icon");
            icon.setAttribute("aria-hidden", "true");
        });
    }

    function setupChildActive(nav) {

        if (nav.dataset.xonChildActiveBound === "true") {
            return;
        }

        nav.dataset.xonChildActiveBound = "true";

        nav.addEventListener("click", function (e) {

            const link = e.target.closest(
                "ul.dropdown-menu > li > a.dropdown-item, " +
                "ul.submenu > li > a.dropdown-item"
            );

            if (!link || !nav.contains(link)) {
                return;
            }

            /*
             * Parent and leaf clicks use the same hierarchy function.
             * Existing menu open/route behavior is not prevented here.
             */
            if (typeof window.xonSetHierarchicalActive === "function") {
                window.xonSetHierarchicalActive(link);
            }

        }, false);
    }

    function initChildMenuIcons() {
        const nav =
            document.querySelector("#main_nav > .navbar-nav");

        if (!nav) {
            return;
        }

        addChildIcons(nav);
        setupChildActive(nav);

        /*
         * Some menus can be rendered after initialization.
         * Re-apply icons without changing existing menu behaviour.
         */
        const observer = new MutationObserver(function () {
            addChildIcons(nav);
        });

        observer.observe(nav, {
            childList: true,
            subtree: true
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            initChildMenuIcons
        );
    } else {
        initChildMenuIcons();
    }
})();


(function () {
    "use strict";

    function directMenu(li) {
        return li.querySelector(":scope > ul.dropdown-menu, :scope > ul.submenu");
    }

    function directLink(li) {
        return li.querySelector(":scope > a.dropdown-item, :scope > a.nav-link");
    }

    function hasMenu(li) {
        return !!directMenu(li);
    }

    function arrowFor(li) {
        var link = directLink(li);
        if (!link) return null;
        var a = link.querySelector(":scope > .xon-tree-arrow");
        if (!a) {
            a = document.createElement("span");
            a.className = "xon-tree-arrow";
            link.appendChild(a);
        }
        return a;
    }

    function setOpen(li, open) {
        var menu = directMenu(li);
        if (!menu) return;

        li.classList.toggle("xon-tree-open", open);
        menu.classList.toggle("xon-tree-open", open);
        menu.classList.remove("show");
        menu.setAttribute("aria-hidden", open ? "false" : "true");

        var arrow = arrowFor(li);
        if (arrow) arrow.textContent = open ? "−" : "+";
    }

    function closeDescendants(li) {
        li.querySelectorAll("li.xon-tree-open").forEach(function (child) {
            if (child === li) return;
            var menu = directMenu(child);
            child.classList.remove("xon-tree-open");
            if (menu) {
                menu.classList.remove("xon-tree-open", "show");
                menu.setAttribute("aria-hidden", "true");
            }
            var arrow = arrowFor(child);
            if (arrow) arrow.textContent = "+";
        });
    }

    function initTree() {
        var nav = document.querySelector("#main_nav > .navbar-nav");
        if (!nav) return;

        // Prepare every real parent, including ADMIN -> User -> nested menus.
        nav.querySelectorAll("li").forEach(function (li) {
            if (!hasMenu(li)) return;
            li.classList.add("xon-tree-parent");
            setOpen(li, false);
        });

        // Do NOT open any top-level menu initially.
        // Menus open only when the user clicks them or when search finds a match.
        nav.querySelectorAll("li.xon-tree-parent").forEach(function (li) {
            setOpen(li, false);
        });

        // =========================================================
        // XON ERP - REMEMBER OPEN SIDEBAR BRANCH
        // =========================================================
        // This is intentionally local to initTree() so existing
        // menu/favourite/route code is not replaced.
        const XON_SIDEBAR_BRANCH_KEY = "__XON_ERP_OPEN_BRANCH__";

        function getStableMenuId(link) {
            if (!link) return null;

            const menuKey =
                link.getAttribute("data-menu-key") ||
                link.getAttribute("data-menukey") ||
                link.dataset.menuKey;

            if (menuKey) {
                return {
                    type: "key",
                    value: menuKey.toString().trim()
                };
            }

            const href =
                (link.getAttribute("href") || "").trim();

            if (
                href &&
                href !== "#" &&
                !href.toLowerCase().startsWith("javascript:")
            ) {
                try {
                    const u = new URL(
                        href,
                        window.location.origin
                    );

                    return {
                        type: "path",
                        value: (
                            u.pathname || "/"
                        )
                            .toLowerCase()
                            .replace(/\/+$/, "") || "/"
                    };
                } catch (_) { }
            }

            const dashboardTab =
                link.getAttribute("data-dashboard-tab");

            if (dashboardTab) {
                return {
                    type: "dashboard",
                    value: dashboardTab.toString().trim()
                };
            }

            const text =
                (link.textContent || "")
                    .replace(/\s+/g, " ")
                    .trim();

            return text
                ? {
                    type: "text",
                    value: text.toLowerCase()
                }
                : null;
        }

        function sameStableMenuId(link, saved) {
            if (!link || !saved) return false;

            const current =
                getStableMenuId(link);

            if (!current) return false;

            if (
                current.type === saved.type &&
                current.value === saved.value
            ) {
                return true;
            }

            /*
             * formKey/query-string changes must NOT make the same
             * sidebar menu look like a different menu.
             */
            if (saved.type === "path") {
                const href =
                    (link.getAttribute("href") || "").trim();

                if (href) {
                    try {
                        const u = new URL(
                            href,
                            window.location.origin
                        );

                        const path =
                            (
                                u.pathname || "/"
                            )
                                .toLowerCase()
                                .replace(/\/+$/, "") || "/";

                        return path === saved.value;
                    } catch (_) { }
                }
            }

            return false;
        }

        function getDirectTreeLink(li) {
            if (!li) return null;

            return li.querySelector(
                ":scope > a.nav-link, " +
                ":scope > a.dropdown-item, " +
                ":scope > a"
            );
        }

        function getTreeChainFromLink(link) {
            const chain = [];
            let li =
                link &&
                link.closest("li");

            while (
                li &&
                nav.contains(li)
            ) {
                const direct =
                    getDirectTreeLink(li);

                if (direct) {
                    chain.unshift(direct);
                }

                if (
                    li.parentElement === nav
                ) {
                    break;
                }

                li =
                    li.parentElement &&
                    li.parentElement.closest("li");
            }

            return chain;
        }

        function saveCurrentSidebarBranch() {
            /*
             * The leaf is the deepest currently active menu.
             * This works for:
             * ADMIN > User
             * ADMIN > User > User Dashboard
             * ADMIN > User > User > Child
             */
            let leaf =
                nav.querySelector(
                    "a.xon-child-child-active"
                );

            if (!leaf) {
                leaf =
                    nav.querySelector(
                        "a.xon-child-active"
                    );
            }

            if (!leaf) {
                leaf =
                    nav.querySelector(
                        "a.xon-parent-active"
                    );
            }

            /*
             * If custom classes are not available yet, use the
             * actual open tree branch as fallback.
             */
            if (!leaf) {
                const openLeaf =
                    Array.from(
                        nav.querySelectorAll(
                            "li.xon-tree-open > a"
                        )
                    ).pop();

                if (openLeaf) {
                    leaf = openLeaf;
                }
            }

            if (!leaf) {
                return;
            }

            const chain =
                getTreeChainFromLink(
                    leaf
                )
                    .map(getStableMenuId)
                    .filter(Boolean);

            if (!chain.length) {
                return;
            }

            try {
                sessionStorage.setItem(
                    XON_SIDEBAR_BRANCH_KEY,
                    JSON.stringify({
                        chain: chain,
                        savedAt: Date.now()
                    })
                );
            } catch (_) {
                /*
                 * Never let storage problems affect navigation.
                 */
            }
        }

        function readSavedSidebarBranch() {
            try {
                const raw =
                    sessionStorage.getItem(
                        XON_SIDEBAR_BRANCH_KEY
                    );

                if (!raw) {
                    return null;
                }

                const data =
                    JSON.parse(raw);

                if (
                    !data ||
                    !Array.isArray(data.chain) ||
                    !data.chain.length
                ) {
                    return null;
                }

                return data;
            } catch (_) {
                return null;
            }
        }

        function findSavedChild(container, savedId) {
            if (!container || !savedId) {
                return null;
            }

            let items = [];

            if (
                container === nav
            ) {
                items =
                    Array.from(
                        nav.children
                    ).filter(
                        function (el) {
                            return (
                                el.tagName === "LI"
                            );
                        }
                    );
            } else {
                const menu =
                    container.querySelector(
                        ":scope > ul.dropdown-menu, " +
                        ":scope > ul.submenu"
                    );

                if (menu) {
                    items =
                        Array.from(
                            menu.children
                        ).filter(
                            function (el) {
                                return (
                                    el.tagName === "LI"
                                );
                            }
                        );
                }
            }

            /*
             * Fallback for DB-generated wrapper elements.
             */
            if (!items.length) {
                items =
                    Array.from(
                        container.querySelectorAll("li")
                    ).filter(
                        function (li) {
                            return (
                                li.parentElement &&
                                li.parentElement.closest("li") ===
                                container
                            );
                        }
                    );
            }

            for (
                let i = 0;
                i < items.length;
                i++
            ) {
                const link =
                    getDirectTreeLink(
                        items[i]
                    );

                if (
                    link &&
                    sameStableMenuId(
                        link,
                        savedId
                    )
                ) {
                    return items[i];
                }
            }

            return null;
        }

        function restoreSavedSidebarBranch() {
            const saved =
                readSavedSidebarBranch();

            if (
                !saved ||
                !saved.chain ||
                !saved.chain.length
            ) {
                return false;
            }

            let container = nav;
            const resolved = [];

            for (
                let i = 0;
                i < saved.chain.length;
                i++
            ) {
                const li =
                    findSavedChild(
                        container,
                        saved.chain[i]
                    );

                if (!li) {
                    break;
                }

                resolved.push(li);
                container = li;
            }

            if (!resolved.length) {
                return false;
            }

            /*
             * Do not clear unrelated Bootstrap state here.
             * Only restore our tree-open/active classes.
             */
            resolved.forEach(
                function (li, index) {
                    const link =
                        getDirectTreeLink(li);

                    if (!link) return;

                    link.classList.add("active");

                    if (index === 0) {
                        link.classList.add(
                            "xon-parent-active"
                        );
                    } else if (
                        index ===
                        resolved.length - 1 &&
                        index >= 2
                    ) {
                        link.classList.add(
                            "xon-child-child-active"
                        );
                    } else {
                        link.classList.add(
                            "xon-child-active"
                        );
                    }

                    /*
                     * IMPORTANT:
                     * Open every ancestor in the saved branch.
                     */
                    if (
                        index <
                        resolved.length - 1
                    ) {
                        setOpen(
                            li,
                            true
                        );

                        li.classList.add(
                            "xon-tree-active-parent"
                        );
                    }
                }
            );

            /*
             * If the final resolved item is itself a parent,
             * keep it open too.
             */
            const last =
                resolved[
                resolved.length - 1
                ];

            if (
                last &&
                hasMenu(last)
            ) {
                setOpen(
                    last,
                    true
                );
            }

            window.setTimeout(
                function () {
                    if (
                        typeof window.xonUpdateBreadcrumb ===
                        "function"
                    ) {
                        window.xonUpdateBreadcrumb();
                    }
                },
                50
            );

            return true;
        }

        /*
         * Save the branch BEFORE any page-internal navigation.
         *
         * Capture phase is intentional:
         * page buttons/links may call preventDefault,
         * stopPropagation, AJAX, location.href, etc.
         * We must save the active sidebar branch first.
         */
        function bindSidebarBranchMemory() {
            if (
                document.documentElement.dataset
                    .xonSidebarBranchMemoryBound ===
                "true"
            ) {
                return;
            }

            document.documentElement.dataset
                .xonSidebarBranchMemoryBound =
                "true";

            document.addEventListener(
                "click",
                function (e) {
                    const target =
                        e.target.closest(
                            "a, button, input[type='button'], " +
                            "input[type='submit']"
                        );

                    if (!target) {
                        return;
                    }

                    /*
                     * Do not save when clicking the sidebar itself.
                     * The existing sidebar click handler will update
                     * the active branch and this listener must not
                     * capture the old branch.
                     */
                    if (
                        target.closest(
                            "#main_nav"
                        )
                    ) {
                        return;
                    }

                    /*
                     * Save the currently open/active sidebar branch
                     * immediately, BEFORE page navigation.
                     */
                    saveCurrentSidebarBranch();
                },
                true
            );
        }

        bindSidebarBranchMemory();

        // Capture parent clicks before Bootstrap.
        function restoreActiveTree() {

            const nav =
                document.querySelector("#main_nav > .navbar-nav");

            if (!nav) {
                return;
            }

            const currentPath =
                window.location.pathname
                    .toLowerCase()
                    .replace(/\/+$/, "");

            if (!currentPath || currentPath === "/") {
                return;
            }

            let activeLink = null;

            // =====================================================
            // FIND CURRENT PAGE FROM MENU URL
            // =====================================================

            nav.querySelectorAll("a[href]").forEach(function (link) {

                const href =
                    (link.getAttribute("href") || "").trim();

                if (!href || href === "#") {
                    return;
                }

                if (
                    href.toLowerCase().startsWith("javascript:")
                ) {
                    return;
                }

                try {

                    const url =
                        new URL(
                            href,
                            window.location.origin
                        );

                    const menuPath =
                        url.pathname
                            .toLowerCase()
                            .replace(/\/+$/, "");

                    if (menuPath === currentPath) {

                        activeLink = link;

                    }

                }
                catch (error) {

                    console.warn(
                        "Invalid menu URL:",
                        href
                    );

                }

            });


            // =====================================================
            // CURRENT PAGE NOT FOUND
            // =====================================================

            if (!activeLink) {

                console.log(
                    "Breadcrumb: menu not found for",
                    currentPath,
                    "- restoring previous sidebar branch"
                );

                /*
                 * IMPORTANT:
                 * Edit/Add/View pages often are NOT separate
                 * sidebar menu items. In that case there is no
                 * activeLink for the current URL.
                 *
                 * Restore the branch from which the user entered
                 * this internal page instead of leaving ADMIN/User
                 * collapsed.
                 */
                restoreSavedSidebarBranch();

                return;
            }


            console.log(
                "Breadcrumb active menu:",
                activeLink
            );

            /*
             * Keep the saved branch synchronized with the actual
             * sidebar page whenever the current URL IS a menu item.
             */
            try {
                const currentChain =
                    getTreeChainFromLink(
                        activeLink
                    )
                        .map(getStableMenuId)
                        .filter(Boolean);

                if (currentChain.length) {
                    sessionStorage.setItem(
                        XON_SIDEBAR_BRANCH_KEY,
                        JSON.stringify({
                            chain: currentChain,
                            savedAt: Date.now()
                        })
                    );
                }
            } catch (_) { }


            // =====================================================
            // SET ADMIN > USER > PAGE ACTIVE
            // =====================================================

            if (
                typeof window.xonSetHierarchicalActive ===
                "function"
            ) {

                window.xonSetHierarchicalActive(
                    activeLink
                );

            }


            // =====================================================
            // OPEN ALL PARENT MENUS
            // =====================================================

            let parent =
                activeLink.parentElement;


            while (
                parent &&
                nav.contains(parent)
            ) {

                if (
                    parent.tagName === "LI" &&
                    typeof hasMenu === "function" &&
                    hasMenu(parent)
                ) {

                    setOpen(
                        parent,
                        true
                    );

                    parent.classList.add(
                        "xon-tree-active-parent"
                    );

                }


                parent =
                    parent.parentElement &&
                    parent.parentElement.closest("li");

            }


            // =====================================================
            // UPDATE BREADCRUMB
            // =====================================================

            window.setTimeout(function () {

                if (
                    typeof window.xonUpdateBreadcrumb ===
                    "function"
                ) {

                    window.xonUpdateBreadcrumb();

                }

            }, 50);

        }

        // Restore ADMIN/User/etc. after a normal MVC page navigation.
        restoreActiveTree();

        /* =========================================================
           XON ERP - STABLE SIDEBAR TREE CLICK HANDLER
           ---------------------------------------------------------
           One delegated handler owns ALL parent expansion.
           Leaf links keep normal navigation.
           ========================================================= */
        nav.querySelectorAll("a[data-bs-toggle], a[data-toggle]").forEach(function (link) {
            link.removeAttribute("data-bs-toggle");
            link.removeAttribute("data-toggle");
            link.removeAttribute("aria-expanded");
        });

        function closeBranch(li) {
            if (!li) return;

            li.classList.remove("xon-tree-open", "xon-tree-click-active");

            const menu = directMenu(li);
            if (menu) {
                menu.classList.remove("xon-tree-open", "show");
                menu.setAttribute("aria-hidden", "true");
            }

            const arrow = li.querySelector(":scope > a > .xon-tree-arrow");
            if (arrow) arrow.textContent = "+";

            li.querySelectorAll("li.xon-tree-open").forEach(function (child) {
                child.classList.remove("xon-tree-open", "xon-tree-click-active");
                const childMenu = directMenu(child);
                if (childMenu) {
                    childMenu.classList.remove("xon-tree-open", "show");
                    childMenu.setAttribute("aria-hidden", "true");
                }
                const childArrow = child.querySelector(":scope > a > .xon-tree-arrow");
                if (childArrow) childArrow.textContent = "▸";
            });
        }

        function openBranch(li) {
            if (!li) return;

            const menu = directMenu(li);
            if (!menu) return;

            li.classList.add("xon-tree-open", "xon-tree-click-active");
            menu.classList.add("xon-tree-open");
            menu.classList.remove("show");
            menu.setAttribute("aria-hidden", "false");

            const arrow = li.querySelector(":scope > a > .xon-tree-arrow");
            if (arrow) arrow.textContent = "−";
        }

        function closeSameLevel(li) {
            const parent = li && li.parentElement;
            if (!parent) return;

            Array.from(parent.children).forEach(function (sibling) {
                if (sibling === li || sibling.tagName !== "LI") return;
                if (sibling.classList.contains("xon-tree-parent")) {
                    closeBranch(sibling);
                }
            });
        }

        function openAncestors(li) {
            let current = li;

            while (current && nav.contains(current)) {
                if (current.tagName === "LI" && hasMenu(current)) {
                    openBranch(current);
                }

                if (current.parentElement === nav) break;

                current = current.parentElement
                    ? current.parentElement.closest("li")
                    : null;
            }
        }

        /* Prepare arrows and mark every real parent. */
        nav.querySelectorAll("li").forEach(function (li) {
            if (!hasMenu(li)) return;

            li.classList.add("xon-tree-parent");

            const link = directLink(li);
            if (!link) return;

            link.removeAttribute("data-bs-toggle");
            link.removeAttribute("data-toggle");
            link.removeAttribute("aria-expanded");

            let arrow = link.querySelector(":scope > .xon-tree-arrow");
            if (!arrow) {
                arrow = document.createElement("span");
                arrow.className = "xon-tree-arrow";
                arrow.textContent = li.classList.contains("xon-tree-open") ? "−" : "+";
                const firstIcon = link.querySelector(":scope > i:not(.xon-tree-arrow)");
                if (firstIcon) {
                    link.insertBefore(arrow, firstIcon);
                } else {
                    link.insertBefore(arrow, link.firstChild);
                }
            }

            /* Do not leave Bootstrap's .show state on a tree menu. */
            closeBranch(li);
        });

        /* Remove Bootstrap dropdown state when it tries to appear. */
        nav.addEventListener("show.bs.dropdown", function (e) {
            const li = e.target && e.target.closest
                ? e.target.closest("li")
                : null;

            if (li && hasMenu(li)) {
                e.preventDefault();
            }
        }, true);

        nav.addEventListener("click", function (e) {
            const link = e.target.closest
                ? e.target.closest("a.nav-link, a.dropdown-item, a.xon-tree-parent-link")
                : null;

            if (!link || !nav.contains(link)) return;

            const li = link.closest("li");
            if (!li) return;

            const menu = directMenu(li);

            /* -----------------------------------------------
               PARENT: toggle only, never navigate.
               ----------------------------------------------- */
            if (menu) {
                e.preventDefault();
                e.stopPropagation();

                const wasOpen = li.classList.contains("xon-tree-open");

                if (wasOpen) {
                    closeBranch(li);
                } else {
                    closeSameLevel(li);
                    openBranch(li);
                    openAncestors(li);
                }

                if (typeof window.xonSetHierarchicalActive === "function") {
                    window.xonSetHierarchicalActive(link);
                }

                if (typeof window.xonUpdateBreadcrumb === "function") {
                    window.xonUpdateBreadcrumb();
                }

                return;
            }

            /* -----------------------------------------------
               LEAF: keep normal navigation but save active tree.
               ----------------------------------------------- */
            if (typeof window.xonSetHierarchicalActive === "function") {
                window.xonSetHierarchicalActive(link);
            }

            openAncestors(li);
        }, true);

        /* Re-prepare menus if Razor/AJAX inserts them later. */
        const treeObserver = new MutationObserver(function () {
            nav.querySelectorAll("li").forEach(function (li) {
                if (!hasMenu(li)) return;

                li.classList.add("xon-tree-parent");

                const link = directLink(li);
                if (!link) return;

                link.removeAttribute("data-bs-toggle");
                link.removeAttribute("data-toggle");

                let arrow = link.querySelector(":scope > .xon-tree-arrow");
                if (!arrow) {
                    arrow = document.createElement("span");
                    arrow.className = "xon-tree-arrow";
                    arrow.textContent = li.classList.contains("xon-tree-open") ? "−" : "+";
                    const firstIcon = link.querySelector(":scope > i:not(.xon-tree-arrow), :scope > .xon-child-icon");
                    if (firstIcon) {
                        link.insertBefore(arrow, firstIcon);
                    } else {
                        link.insertBefore(arrow, link.firstChild);
                    }
                }
            });
        });

        treeObserver.observe(nav, {
            childList: true,
            subtree: true
        });

        initSearch(nav);
    }

    function initSearch(nav) {
        var input = document.getElementById("xonMenuSearch");
        var clear = document.getElementById("xonMenuSearchClear");

        if (!input) return;

        // =========================================================
        // XON ERP - SIDEBAR SEARCH PERSISTENCE
        // ---------------------------------------------------------
        // Keeps the search text/filter while navigating between MVC
        // pages in the same browser tab.
        //
        // IMPORTANT:
        // - sessionStorage is used so the search survives page
        //   navigation and refresh, but resets when the tab is closed.
        // - Search is restored after the new page's sidebar is ready.
        // - Clearing the search removes the saved value.
        // =========================================================
        const XON_MENU_SEARCH_KEY = "__XON_ERP_MENU_SEARCH__";

        // Prevent duplicate initialization if initTree() is called again.
        if (input.dataset.xonSearchInitialized === "true") {
            restoreMenuSearch();
            return;
        }

        input.dataset.xonSearchInitialized = "true";

        // Prevent browser autofill / restore of unrelated values.
        input.setAttribute("autocomplete", "off");
        input.setAttribute("autocorrect", "off");
        input.setAttribute("autocapitalize", "off");
        input.setAttribute("spellcheck", "false");

        // Keep readonly initially to prevent unwanted browser autofill.
        input.setAttribute("readonly", "readonly");

        // When the user clicks/focuses the search box, allow typing.
        input.addEventListener("focus", function () {
            input.removeAttribute("readonly");
        });


        // ============================================
        // SAVE SEARCH VALUE
        // ============================================

        function saveMenuSearch() {
            try {
                var value = (input.value || "").trim();

                if (value) {
                    sessionStorage.setItem(
                        XON_MENU_SEARCH_KEY,
                        value
                    );
                } else {
                    sessionStorage.removeItem(
                        XON_MENU_SEARCH_KEY
                    );
                }
            } catch (e) {
                // Storage problems must never break navigation.
            }
        }


        // ============================================
        // READ SAVED SEARCH
        // ============================================

        function getSavedMenuSearch() {
            try {
                return (
                    sessionStorage.getItem(
                        XON_MENU_SEARCH_KEY
                    ) || ""
                ).trim();
            } catch (e) {
                return "";
            }
        }


        // ============================================
        // GET MENU SEARCH TEXT
        // ============================================
        // Search MUST use the DB SearchColumn value.
        // Do not search the truncated/visible DisplayMainMenuHeading.

        function rowSearchText(li) {
            var link = directLink(li);

            if (!link) return "";

            var searchColumn =
                (link.getAttribute("data-search-column") || "")
                    .replace(/\s+/g, " ")
                    .trim();

            return searchColumn.toLowerCase();
        }


        // ============================================
        // SHOW ALL MENU ITEMS
        // ============================================

        function showAll() {

            nav.querySelectorAll("li.xon-search-hidden")
                .forEach(function (li) {
                    li.classList.remove("xon-search-hidden");
                });

            nav.querySelectorAll("li.xon-search-match")
                .forEach(function (li) {
                    li.classList.remove("xon-search-match");
                });

            // Close tree menus when search is empty.
            nav.querySelectorAll("li.xon-tree-parent")
                .forEach(function (li) {

                    li.classList.remove(
                        "xon-tree-open",
                        "xon-tree-click-active",
                        "xon-tree-active-parent"
                    );

                    var menu = directMenu(li);

                    if (menu) {
                        menu.classList.remove(
                            "xon-tree-open",
                            "show"
                        );

                        menu.setAttribute(
                            "aria-hidden",
                            "true"
                        );
                    }

                    var arrow = li.querySelector(
                        ":scope > a > .xon-tree-arrow"
                    );

                    if (arrow) {
                        arrow.textContent = "+";
                    }
                });

            // Remove search-specific active classes.
            nav.querySelectorAll(
                ".xon-parent-active, " +
                ".xon-child-active, " +
                ".xon-child-child-active"
            ).forEach(function (el) {

                el.classList.remove(
                    "xon-parent-active",
                    "xon-child-active",
                    "xon-child-child-active"
                );
            });

            if (clear) {
                clear.style.display = "none";
            }

            if (typeof setExpandCollapseButtonState === "function") {
                setExpandCollapseButtonState(false, false);
            }
        }


        // ============================================
        // CLEAR SEARCH
        // ============================================

        function clearSearch(removeSavedValue) {

            input.value = "";

            showAll();

            if (clear) {
                clear.style.display = "none";
            }

            if (removeSavedValue !== false) {
                try {
                    sessionStorage.removeItem(
                        XON_MENU_SEARCH_KEY
                    );
                } catch (e) {
                    // Ignore storage errors.
                }
            }
        }


        // ============================================
        // SEARCH
        // ============================================

        function search() {

            var q = (input.value || "")
                .trim()
                .toLowerCase();

            // If search box is empty, show complete menu.
            if (!q) {
                showAll();
                return;
            }

            if (clear) {
                clear.style.display = "block";
            }

            var items = Array.from(
                nav.querySelectorAll("li")
            );

            // Hide all menu items first.
            items.forEach(function (li) {

                li.classList.add(
                    "xon-search-hidden"
                );

                li.classList.remove(
                    "xon-search-match"
                );
            });

            // Find matching menu items.
            items.forEach(function (li) {

                // Search only against DB DynamicMenu.SearchColumn.
                // Example: typing "alter" matches SearchColumn =
                // "Alternate Item Master" even if the visible label is
                // shortened to "Alternate Item Mas..." by CSS.
                var text = rowSearchText(li);

                if (!text || text.indexOf(q) < 0) {
                    return;
                }

                // Show matching item.
                li.classList.remove(
                    "xon-search-hidden"
                );

                li.classList.add(
                    "xon-search-match"
                );

                // Show all parent menus.
                var p =
                    li.parentElement &&
                    li.parentElement.closest("li");

                while (
                    p &&
                    nav.contains(p)
                ) {

                    p.classList.remove(
                        "xon-search-hidden"
                    );

                    if (hasMenu(p)) {
                        setOpen(p, true);
                    }

                    p =
                        p.parentElement &&
                        p.parentElement.closest("li");
                }
            });

            if (typeof setExpandCollapseButtonState === "function") {
                setExpandCollapseButtonState(false, true);
            }
        }


        // ============================================
        // RESTORE SEARCH AFTER PAGE NAVIGATION
        // ============================================

        function restoreMenuSearch() {

            var savedValue = getSavedMenuSearch();

            if (!savedValue) {
                // No saved search: make sure the box is empty.
                input.value = "";
                input.setAttribute("readonly", "readonly");
                showAll();
                return;
            }

            // Restore exactly what the user searched for.
            input.removeAttribute("readonly");
            input.value = savedValue;

            // Re-apply the filter against the newly rendered menu.
            search();
        }


        // ============================================
        // INITIAL STATE
        // ============================================

        // IMPORTANT:
        // Do NOT call clearSearch() here because it would destroy the
        // search stored before an MVC page navigation.
        var savedSearch = getSavedMenuSearch();

        if (savedSearch) {
            input.removeAttribute("readonly");
            input.value = savedSearch;
            search();
        } else {
            input.value = "";
            showAll();
        }


        // ============================================
        // USER TYPES IN SEARCH
        // ============================================

        input.addEventListener("input", function () {

            saveMenuSearch();

            search();
        });


        // ============================================
        // ESC = CLEAR SEARCH
        // ============================================

        input.addEventListener("keydown", function (e) {

            if (e.key === "Escape") {

                clearSearch(true);

                input.blur();

                input.setAttribute(
                    "readonly",
                    "readonly"
                );
            }
        });


        // ============================================
        // CLEAR BUTTON
        // ============================================

        if (clear) {

            clear.addEventListener("click", function () {

                clearSearch(true);

                input.focus();
            });
        }


        // ============================================
        // PAGE NAVIGATION / PAGE RESTORE
        // ============================================

        // Do NOT clear the search on pageshow.
        // The previous implementation did that and caused the search
        // to disappear every time an MVC page was opened.
        window.addEventListener("pageshow", function () {

            // Give the new page/sidebar time to render its database menus.
            window.setTimeout(function () {
                restoreMenuSearch();
            }, 0);

            window.setTimeout(function () {
                restoreMenuSearch();
            }, 150);

            window.setTimeout(function () {
                restoreMenuSearch();
            }, 400);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initTree);
    } else {
        initTree();
    }
})();

/* =========================================================
   XON ERP - FINAL BREADCRUMB REFRESH
   Handles MVC page navigation/menu rendering timing.
   ========================================================= */
(function () {

    function refreshXonBreadcrumb() {

        if (typeof window.xonUpdateBreadcrumb === "function") {
            window.xonUpdateBreadcrumb();
        }
    }

    if (document.readyState === "loading") {

        document.addEventListener(
            "DOMContentLoaded",
            function () {
                window.setTimeout(refreshXonBreadcrumb, 500);
            }
        );

    } else {

        window.setTimeout(refreshXonBreadcrumb, 500);
    }

})();


/* =========================================================
   XON ERP - AUTO EXPAND SIDEBAR ON MOUSE HOVER
   ========================================================= */
(function () {
    "use strict";

    function initSidebarHoverExpand() {
        const sidebar = document.getElementById("main_nav");

        if (!sidebar) {
            return;
        }

        /* Hover expand is for desktop only. */
        if (window.innerWidth <= 700) {
            return;
        }

        sidebar.addEventListener("mouseenter", function () {
            /* Only auto-expand when the page is in collapsed state. */
            if (sidebar.classList.contains("erp-sidebar-collapsed")) {
                sidebar.classList.add("erp-sidebar-hover-expanded");
                document.body.classList.add("erp-sidebar-hover-expanded");
            }
        });

        sidebar.addEventListener("mouseleave", function () {
            sidebar.classList.remove("erp-sidebar-hover-expanded");
            document.body.classList.remove("erp-sidebar-hover-expanded");
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initSidebarHoverExpand);
    } else {
        initSidebarHoverExpand();
    }
})();
/* =========================================================
   XON ERP - BREADCRUMB MENU FINAL FIX
   ---------------------------------------------------------
   Fixes:
   1. Breadcrumb arrow opens the REAL DB menu tree.
   2. Child -> child menu opens correctly.
   3. Each level has its own scroll area.
   4. Nested menus are opened by CLICK, not hover only.
   5. Popup is positioned inside the viewport.
   6. No menu names are hard-coded.
   7. Existing #main_nav menu / routes / permissions are reused.
   ========================================================= */
(function () {
    "use strict";

    let popup = null;
    let outsideHandlerBound = false;

    const STYLE_ID = "xon-breadcrumb-menu-final-style";

    function textOf(el) {
        return (el?.textContent || "")
            .replace(/\s+/g, " ")
            .trim();
    }

    function getDirectLink(li) {
        if (!li) return null;

        return li.querySelector(
            ":scope > a, :scope > button, :scope > .nav-link"
        );
    }

    /*
     * IMPORTANT:
     * Do not depend on Bootstrap's exact submenu wrapper.
     * In this ERP some DB-generated menus can have extra wrappers/classes.
     * We therefore discover the REAL immediate children by tree position:
     *   parent LI -> child LI -> grand-child LI
     * This keeps the menu 100% database/DOM driven.
     */
    function getDirectChildItems(li) {
        if (!li) return [];

        /* Dashboard uses buttons inside .mis-dashboard-submenu. */
        const dashboardChildren = li.querySelectorAll(
            ":scope > .mis-dashboard-submenu .mis-dashboard-child"
        );

        if (dashboardChildren.length) {
            return Array.from(dashboardChildren);
        }

        /*
         * Find LI descendants whose nearest LI ancestor is this LI.
         * This works even if the submenu has a div/ul wrapper in between.
         */
        return Array.from(li.querySelectorAll("li")).filter(function (childLi) {
            return childLi.parentElement &&
                childLi.parentElement.closest("li") === li;
        });
    }

    function getDirectChildMenu(li) {
        if (!li) return null;

        return getDirectChildItems(li).length > 0
            ? li
            : null;
    }

    function closePopup() {
        if (popup) {
            popup.remove();
            popup = null;
        }
    }

    function injectStyle() {
        if (document.getElementById(STYLE_ID)) {
            return;
        }

        const style = document.createElement("style");
        style.id = STYLE_ID;

        style.textContent = `
            /* =====================================================
               BREADCRUMB POPUP ROOT
               ===================================================== */
            .xon-breadcrumb-popup-final {
                position: fixed !important;
                z-index: 2147483000 !important;
                display: flex !important;
                flex-direction: column !important;
                box-sizing: border-box !important;
                width: 286px !important;
                max-width: calc(100vw - 16px) !important;
                height: auto !important;
                max-height: calc(100vh - 24px) !important;
                padding: 6px !important;
                background: #fff !important;
                border: 1px solid #dfe8e9 !important;
                border-radius: 9px !important;
                box-shadow: 0 12px 32px rgba(0, 45, 50, .20) !important;
                overflow: visible !important;
                font-family: Arial, Helvetica, sans-serif !important;
            }

            .xon-breadcrumb-popup-title {
                flex: 0 0 auto !important;
                min-height: 30px !important;
                display: flex !important;
                align-items: center !important;
                padding: 3px 9px 5px !important;
                color: #075b60 !important;
                font-size: 12px !important;
                font-weight: 700 !important;
                border-bottom: 1px solid #edf2f2 !important;
                box-sizing: border-box !important;
            }

            .xon-breadcrumb-popup-list {
                flex: 1 1 auto !important;
                min-height: 0 !important;
                overflow-y: auto !important;
                overflow-x: hidden !important;
                max-height: calc(100vh - 90px) !important;
                padding: 4px 2px !important;
                margin: 0 !important;
                list-style: none !important;
                scrollbar-width: thin !important;
                scrollbar-color: #aebfc1 transparent !important;
            }

            .xon-breadcrumb-popup-list::-webkit-scrollbar {
                width: 7px !important;
            }

            .xon-breadcrumb-popup-list::-webkit-scrollbar-thumb {
                background: #aebfc1 !important;
                border-radius: 10px !important;
            }

            .xon-breadcrumb-popup-list::-webkit-scrollbar-track {
                background: transparent !important;
            }

            .xon-breadcrumb-popup-row {
                position: relative !important;
                width: 100% !important;
                min-height: 36px !important;
                box-sizing: border-box !important;
                display: flex !important;
                align-items: center !important;
                gap: 8px !important;
                padding: 4px 7px !important;
                margin: 1px 0 !important;
                border: 0 !important;
                border-radius: 6px !important;
                background: transparent !important;
                color: #425356 !important;
                font-size: 12px !important;
                font-weight: 700 !important;
                text-align: left !important;
                cursor: pointer !important;
                user-select: none !important;
                box-sizing: border-box !important;
            }

            .xon-breadcrumb-popup-row:hover,
            .xon-breadcrumb-popup-row.is-open {
                background: #edf7f7 !important;
                color: #075b60 !important;
            }

            .xon-breadcrumb-popup-row-icon {
                flex: 0 0 27px !important;
                width: 27px !important;
                height: 27px !important;
                display: inline-flex !important;
                align-items: center !important;
                justify-content: center !important;
                border-radius: 6px !important;
                background: #edf7f7 !important;
                color: #075b60 !important;
                font-size: 11px !important;
            }

            .xon-breadcrumb-popup-row:hover .xon-breadcrumb-popup-row-icon,
            .xon-breadcrumb-popup-row.is-open .xon-breadcrumb-popup-row-icon {
                background: #075b60 !important;
                color: #fff !important;
            }

            .xon-breadcrumb-popup-row-text {
                flex: 1 1 auto !important;
                min-width: 0 !important;
                overflow: hidden !important;
                text-overflow: ellipsis !important;
                white-space: nowrap !important;
            }

            .xon-breadcrumb-popup-row-arrow {
                flex: 0 0 auto !important;
                width: 16px !important;
                text-align: center !important;
                color: #718184 !important;
                font-size: 10px !important;
            }

            .xon-breadcrumb-popup-row.is-open .xon-breadcrumb-popup-row-arrow {
                color: #075b60 !important;
            }

            /* Nested level. It is NOT clipped by the parent scroll list. */
            .xon-breadcrumb-popup-level {
                position: fixed !important;
                z-index: 2147483001 !important;
                display: flex !important;
                flex-direction: column !important;
                box-sizing: border-box !important;
                width: 286px !important;
                max-width: calc(100vw - 16px) !important;
                height: auto !important;
                max-height: calc(100vh - 24px) !important;
                padding: 6px !important;
                background: #fff !important;
                border: 1px solid #dfe8e9 !important;
                border-radius: 9px !important;
                box-shadow: 0 12px 32px rgba(0, 45, 50, .20) !important;
                overflow: hidden !important;
            }

            .xon-breadcrumb-popup-level-title {
                flex: 0 0 auto !important;
                min-height: 30px !important;
                display: flex !important;
                align-items: center !important;
                padding: 3px 9px 5px !important;
                color: #075b60 !important;
                font-size: 12px !important;
                font-weight: 700 !important;
                border-bottom: 1px solid #edf2f2 !important;
            }

            .xon-breadcrumb-popup-level-list {
                flex: 1 1 auto !important;
                min-height: 0 !important;
                overflow-y: auto !important;
                overflow-x: hidden !important;
                max-height: calc(100vh - 90px) !important;
                padding: 4px 2px !important;
                margin: 0 !important;
                list-style: none !important;
                scrollbar-width: thin !important;
                scrollbar-color: #aebfc1 transparent !important;
            }

            .xon-breadcrumb-popup-level-list::-webkit-scrollbar {
                width: 7px !important;
            }

            .xon-breadcrumb-popup-level-list::-webkit-scrollbar-thumb {
                background: #aebfc1 !important;
                border-radius: 10px !important;
            }

            .xon-breadcrumb-popup-level-list::-webkit-scrollbar-track {
                background: transparent !important;
            }

            .xon-breadcrumb-arrow {
                cursor: pointer !important;
                user-select: none !important;
                border-radius: 5px !important;
            }

            .xon-breadcrumb-arrow:hover,
            .xon-breadcrumb-arrow.is-open {
                background: #edf7f7 !important;
                color: #075b60 !important;
            }

            @media (max-width: 700px) {
                .xon-breadcrumb-popup-final,
                .xon-breadcrumb-popup-level {
                    width: min(280px, calc(100vw - 16px)) !important;
                    height: min(430px, calc(100vh - 20px)) !important;
                }
            }
        `;

        document.head.appendChild(style);
    }

    function getIconFromSource(li) {
        const source = getDirectLink(li);

        if (!source) {
            return "fas fa-folder";
        }

        const icon = source.querySelector(
            ":scope > i:not(.xon-tree-arrow), " +
            ":scope > .xon-child-icon, " +
            ":scope > i"
        );

        if (icon && icon.className) {
            return icon.className;
        }

        return "fas fa-folder";
    }

    function createRow(sourceLi) {
        const row = document.createElement("div");
        row.className = "xon-breadcrumb-popup-row";

        const icon = document.createElement("i");
        icon.className =
            "xon-breadcrumb-popup-row-icon " +
            getIconFromSource(sourceLi);

        /* Prevent the generic icon class from replacing the actual icon. */
        icon.className = getIconFromSource(sourceLi);
        icon.classList.add("xon-breadcrumb-popup-row-icon");

        const text = document.createElement("span");
        text.className = "xon-breadcrumb-popup-row-text";
        text.textContent = textOf(getDirectLink(sourceLi)) || textOf(sourceLi);

        row.appendChild(icon);
        row.appendChild(text);

        const childMenu = getDirectChildMenu(sourceLi);

        if (childMenu) {
            const arrow = document.createElement("span");
            arrow.className = "xon-breadcrumb-popup-row-arrow";
            arrow.textContent = "›";
            row.appendChild(arrow);
        }

        row.__xonSourceLi = sourceLi;

        return row;
    }

    function findOriginalDashboardChild(tabName) {
        const children = document.querySelectorAll(
            "#main_nav .mis-dashboard-submenu .mis-dashboard-child"
        );

        const wanted = (tabName || "")
            .replace(/\s+/g, " ")
            .trim()
            .toLowerCase();

        return Array.from(children).find(function (child) {
            return (
                (child.getAttribute("data-dashboard-tab") || "")
                    .replace(/\s+/g, " ")
                    .trim()
                    .toLowerCase() === wanted
            );
        }) || null;
    }

    function activateSource(sourceLi) {
        if (!sourceLi) return;

        const source = getDirectLink(sourceLi);
        if (!source) return;

        /* Dashboard child buttons are special. */
        if (source.classList.contains("mis-dashboard-child")) {
            const tabName = source.getAttribute("data-dashboard-tab");
            const original = findOriginalDashboardChild(tabName);

            closePopup();

            if (original) {
                original.click();
            }

            return;
        }

        /* A parent with children must be opened, not navigated. */
        if (getDirectChildMenu(sourceLi)) {
            return;
        }

        closePopup();

        if (typeof source.click === "function") {
            source.click();
        }
    }

    function positionLevel(level, anchor, preferRight) {
        const gap = 5;
        const rect = anchor.getBoundingClientRect();

        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        const width = Math.min(286, viewportWidth - 16);
        const maxHeight = Math.min(520, viewportHeight - 24);

        /*
         * IMPORTANT:
         * Do NOT force every popup to 520px.
         * A menu with one child should be a small popup, while a
         * long DB menu should grow only up to maxHeight and then scroll.
         */
        level.style.width = width + "px";
        level.style.height = "auto";
        level.style.maxHeight = maxHeight + "px";

        /* Let the browser calculate the natural content height. */
        const naturalHeight = level.scrollHeight;
        const height = Math.min(
            Math.max(naturalHeight, 48),
            maxHeight
        );

        level.style.height = height + "px";

        let left;

        if (preferRight) {
            left = rect.right + gap;

            if (left + width > viewportWidth - 8) {
                left = rect.left - width - gap;
            }
        } else {
            left = rect.left;
        }

        if (left < 8) {
            left = 8;
        }

        if (left + width > viewportWidth - 8) {
            left = viewportWidth - width - 8;
        }

        let top = rect.top;

        if (top + height > viewportHeight - 8) {
            top = viewportHeight - height - 8;
        }

        if (top < 8) {
            top = 8;
        }

        level.style.left = Math.round(left) + "px";
        level.style.top = Math.round(top) + "px";
    }

    function closeNestedLevels(levelIndex) {
        if (!popup) return;

        popup.querySelectorAll(
            ".xon-breadcrumb-popup-level"
        ).forEach(function (level) {
            if (Number(level.dataset.level || 0) >= levelIndex) {
                level.remove();
            }
        });

        popup.querySelectorAll(
            ".xon-breadcrumb-popup-row.is-open"
        ).forEach(function (row) {
            const rowLevel = Number(row.dataset.level || 0);
            if (rowLevel >= levelIndex) {
                row.classList.remove("is-open");
            }
        });
    }

    function openNestedLevel(sourceLi, anchorRow, levelIndex) {
        const childMenu = getDirectChildMenu(sourceLi);

        if (!childMenu) {
            activateSource(sourceLi);
            return;
        }

        closeNestedLevels(levelIndex);

        const level = document.createElement("div");
        level.className = "xon-breadcrumb-popup-level";
        level.dataset.level = String(levelIndex);

        const title = document.createElement("div");
        title.className = "xon-breadcrumb-popup-level-title";
        title.textContent = textOf(getDirectLink(sourceLi)) || "Menu";

        const list = document.createElement("div");
        list.className = "xon-breadcrumb-popup-level-list";

        getDirectChildItems(sourceLi).forEach(function (childLi) {
            const row = createRow(childLi);
            row.dataset.level = String(levelIndex);
            list.appendChild(row);

            row.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();

                const childSource = row.__xonSourceLi;
                const hasChildren = !!getDirectChildMenu(childSource);

                if (hasChildren) {
                    row.classList.add("is-open");
                    openNestedLevel(
                        childSource,
                        row,
                        levelIndex + 1
                    );
                } else {
                    activateSource(childSource);
                }
            });
        });

        level.appendChild(title);
        level.appendChild(list);

        popup.appendChild(level);

        positionLevel(level, anchorRow, true);
    }

    function getSourceForBreadcrumbItem(itemIndex) {
        const nav = document.querySelector(
            "#main_nav > .navbar-nav"
        );

        if (!nav) return null;

        const activeLinks = [];

        const parent = nav.querySelector(
            "a.xon-parent-active"
        );

        const child = nav.querySelector(
            "a.xon-child-active"
        );

        const childChild = nav.querySelector(
            "a.xon-child-child-active"
        );

        if (parent) activeLinks.push(parent);
        if (child) activeLinks.push(child);
        if (childChild) activeLinks.push(childChild);

        if (!activeLinks[itemIndex]) {
            return null;
        }

        return activeLinks[itemIndex].closest("li");
    }

    function createPopupForSource(sourceLi, arrow, titleText) {
        const childMenu = getDirectChildMenu(sourceLi);

        if (!childMenu) {
            return;
        }

        closePopup();
        injectStyle();

        popup = document.createElement("div");
        popup.className = "xon-breadcrumb-popup-final";

        const title = document.createElement("div");
        title.className = "xon-breadcrumb-popup-title";
        title.textContent = titleText || textOf(getDirectLink(sourceLi));

        const list = document.createElement("div");
        list.className = "xon-breadcrumb-popup-list";

        getDirectChildItems(sourceLi).forEach(function (childLi) {
            const row = createRow(childLi);
            row.dataset.level = "0";
            list.appendChild(row);

            row.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();

                const rowSource = row.__xonSourceLi;
                const hasChildren = !!getDirectChildMenu(rowSource);

                if (hasChildren) {
                    row.classList.add("is-open");
                    openNestedLevel(
                        rowSource,
                        row,
                        1
                    );
                } else {
                    activateSource(rowSource);
                }
            });
        });

        popup.appendChild(title);
        popup.appendChild(list);
        document.body.appendChild(popup);

        /* Arrow itself is the anchor for the first level. */
        positionLevel(popup, arrow, false);

        arrow.classList.add("is-open");
        arrow.setAttribute("aria-expanded", "true");
    }

    function bindArrows() {
        const breadcrumb = document.getElementById("xonBreadcrumb");

        if (!breadcrumb) return;

        const separators = breadcrumb.querySelectorAll(
            ".xon-breadcrumb-separator"
        );

        separators.forEach(function (arrow, separatorIndex) {
            if (arrow.dataset.xonFinalArrowBound === "true") {
                return;
            }

            arrow.dataset.xonFinalArrowBound = "true";
            arrow.classList.add("xon-breadcrumb-arrow");
            arrow.setAttribute("role", "button");
            arrow.setAttribute("tabindex", "0");
            arrow.setAttribute("aria-expanded", "false");

            arrow.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();

                if (
                    popup &&
                    arrow.classList.contains("is-open")
                ) {
                    closePopup();
                    arrow.classList.remove("is-open");
                    arrow.setAttribute("aria-expanded", "false");
                    return;
                }

                document.querySelectorAll(
                    ".xon-breadcrumb-arrow.is-open"
                ).forEach(function (other) {
                    other.classList.remove("is-open");
                    other.setAttribute("aria-expanded", "false");
                });

                /* Dashboard breadcrumb: Dashboard > Inventory Dashboard */
                const breadcrumbItems = breadcrumb.querySelectorAll(
                    ".xon-breadcrumb-item"
                );

                const previousItem =
                    arrow.previousElementSibling;

                const previousText = textOf(previousItem)
                    .toUpperCase();

                if (previousText === "DASHBOARD") {
                    /*
                     * MY FAVOURITES is inserted before Dashboard.
                     * Therefore Dashboard must be found by its label,
                     * not by :first-child.
                     */
                    const dashboardNav =
                        document.querySelector(
                            "#main_nav > .navbar-nav"
                        );

                    const dashboardItem =
                        dashboardNav
                            ? Array.from(
                                dashboardNav.children
                            ).find(function (item) {

                                if (
                                    !item.classList.contains(
                                        "nav-item"
                                    )
                                ) {
                                    return false;
                                }

                                const link =
                                    item.querySelector(
                                        ":scope > .nav-link"
                                    );

                                return (
                                    link &&
                                    (link.textContent || "")
                                        .replace(/\s+/g, " ")
                                        .trim()
                                        .toUpperCase() ===
                                    "DASHBOARD"
                                );
                            })
                            : null;

                    if (dashboardItem) {
                        createPopupForSource(
                            dashboardItem,
                            arrow,
                            "Dashboard"
                        );
                    }

                    return;
                }

                /* Normal hierarchical breadcrumb. */
                const sourceLi =
                    getSourceForBreadcrumbItem(separatorIndex);

                if (!sourceLi) {
                    return;
                }

                createPopupForSource(
                    sourceLi,
                    arrow,
                    previousText
                );
            });

            arrow.addEventListener("keydown", function (e) {
                if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault();
                    arrow.click();
                }
            });
        });
    }

    function refresh() {
        /* Breadcrumb gets rebuilt dynamically, so bindings must be re-applied. */
        window.setTimeout(bindArrows, 50);
        window.setTimeout(bindArrows, 250);
        window.setTimeout(bindArrows, 600);
    }

    if (!outsideHandlerBound) {
        outsideHandlerBound = true;

        document.addEventListener("click", function (e) {
            if (
                e.target.closest(".xon-breadcrumb-popup-final") ||
                e.target.closest(".xon-breadcrumb-popup-level") ||
                e.target.closest(".xon-breadcrumb-arrow")
            ) {
                return;
            }

            closePopup();

            document.querySelectorAll(
                ".xon-breadcrumb-arrow.is-open"
            ).forEach(function (arrow) {
                arrow.classList.remove("is-open");
                arrow.setAttribute("aria-expanded", "false");
            });
        });

        window.addEventListener("resize", function () {
            closePopup();

            document.querySelectorAll(
                ".xon-breadcrumb-arrow.is-open"
            ).forEach(function (arrow) {
                arrow.classList.remove("is-open");
                arrow.setAttribute("aria-expanded", "false");
            });
        });

        window.addEventListener("scroll", function () {
            /* Keep popup stable during page scroll; viewport is recalculated. */
            if (!popup) return;

            const openArrow = document.querySelector(
                ".xon-breadcrumb-arrow.is-open"
            );

            if (!openArrow) return;

            positionLevel(
                popup,
                openArrow,
                false
            );
        }, true);
    }

    /* Observe breadcrumb because xonUpdateBreadcrumb() replaces its HTML. */
    function initObserver() {
        const breadcrumb = document.getElementById("xonBreadcrumb");

        if (!breadcrumb || breadcrumb.dataset.xonFinalObserver === "true") {
            return;
        }

        breadcrumb.dataset.xonFinalObserver = "true";

        const observer = new MutationObserver(function () {
            closePopup();
            bindArrows();
        });

        observer.observe(breadcrumb, {
            childList: true,
            subtree: true
        });

        bindArrows();
    }

    function init() {
        injectStyle();
        initObserver();
        refresh();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }

    window.setTimeout(init, 500);
    window.setTimeout(init, 1000);
    window.setTimeout(init, 1600);

})();

(function () {
    "use strict";

    function findDashboardItem() {

        const nav =
            document.querySelector(
                "#main_nav > .navbar-nav"
            );

        if (!nav) {
            return null;
        }

        return Array.from(nav.children).find(
            function (item) {

                if (
                    !item.classList.contains(
                        "nav-item"
                    )
                ) {
                    return false;
                }

                const link =
                    item.querySelector(
                        ":scope > .nav-link"
                    );

                if (!link) {
                    return false;
                }

                const text =
                    (link.textContent || "")
                        .replace(/\s+/g, " ")
                        .trim()
                        .toUpperCase();

                return text === "DASHBOARD";
            }
        ) || null;
    }

    function isDashboardPage() {

        const path =
            (
                window.location.pathname ||
                "/"
            )
                .toLowerCase()
                .replace(/\/+$/, "");

        return (
            path === "" ||
            path === "/" ||
            path === "/home" ||
            path === "/home/dashboard"
        );
    }

    function keepInitialDashboardCollapsed() {

        if (!isDashboardPage()) {
            return false;
        }

        if (
            window.__xonDashboardUserInteracted ===
            true
        ) {
            return true;
        }

        const dashboardItem =
            findDashboardItem();

        if (!dashboardItem) {
            return false;
        }

        const submenu =
            dashboardItem.querySelector(
                ":scope > .mis-dashboard-submenu"
            );

        if (!submenu) {
            return false;
        }

        dashboardItem.classList.remove(
            "mis-dashboard-expanded"
        );

        submenu.setAttribute(
            "aria-hidden",
            "true"
        );

        submenu.style.removeProperty("display");

        return true;
    }

    function bindRealDashboardClick() {

        const dashboardItem =
            findDashboardItem();

        if (!dashboardItem) {
            return false;
        }

        const dashboardLink =
            dashboardItem.querySelector(
                ":scope > .nav-link"
            );

        if (!dashboardLink) {
            return false;
        }

        if (
            dashboardLink.dataset
                .xonDashboardStateBound ===
            "true"
        ) {
            return true;
        }

        dashboardLink.dataset
            .xonDashboardStateBound =
            "true";

        dashboardLink.addEventListener(
            "click",
            function () {

                window.__xonDashboardUserInteracted =
                    true;

            },
            true
        );

        return true;
    }

    function initDashboardState() {

        bindRealDashboardClick();
        keepInitialDashboardCollapsed();
        [0, 50, 150, 300, 500, 800].forEach(
            function (delay) {

                window.setTimeout(
                    function () {

                        bindRealDashboardClick();
                        keepInitialDashboardCollapsed();

                    },
                    delay
                );
            }
        );
    }

    if (document.readyState === "loading") {

        document.addEventListener(
            "DOMContentLoaded",
            initDashboardState
        );

    } else {

        initDashboardState();

    }

})();

/* =========================================================
   XON ERP - ACTIVE MENU AUTO SCROLL - FINAL
   ---------------------------------------------------------
   Behaviour:
   1. When a page opens, restore/open its real sidebar branch.
   2. Automatically scroll ONLY the sidebar scroll container so
      the currently opened/active menu is visible.
   3. Scrolls the minimum required amount - up OR down.
   4. Never scrolls the main page/window.
   5. When a parent menu is opened, reveal the newly opened branch.
   6. User never needs to manually scroll to find the menu they opened.
   ========================================================= */
(function () {
    "use strict";

    const BOUND_FLAG = "xonFinalActiveMenuAutoScrollBound";

    function getSidebar() {
        return document.getElementById("main_nav");
    }

    function isScrollable(el) {
        if (!el) return false;

        const style = window.getComputedStyle(el);
        const overflowY = style.overflowY;

        return (
            (overflowY === "auto" ||
                overflowY === "scroll" ||
                overflowY === "overlay") &&
            el.scrollHeight > el.clientHeight + 2
        );
    }

    function getScrollContainer() {
        const sidebar = getSidebar();
        if (!sidebar) return null;

        /*
         * In the ERP the scrollable element can be #main_nav OR a
         * child wrapper depending on the layout/version.
         */
        if (isScrollable(sidebar)) {
            return sidebar;
        }

        const candidates = [
            sidebar.querySelector(":scope > .navbar-nav"),
            sidebar.querySelector(".navbar-nav"),
            sidebar.querySelector(".sidebar-menu"),
            sidebar.querySelector(".sidebar-body"),
            sidebar.querySelector(".offcanvas-body")
        ];

        for (const el of candidates) {
            if (isScrollable(el)) {
                return el;
            }
        }

        /* If CSS does not expose overflowY, use the sidebar itself. */
        return sidebar;
    }

    function getDirectMenu(li) {
        if (!li) return null;

        return li.querySelector(
            ":scope > ul.dropdown-menu, :scope > ul.submenu"
        );
    }

    function getDirectLink(li) {
        if (!li) return null;

        return li.querySelector(
            ":scope > a.nav-link, " +
            ":scope > a.dropdown-item, " +
            ":scope > a"
        );
    }

    function getVisibleChildren(li) {
        if (!li) return [];

        /* Dashboard special submenu */
        const dashboardChildren = li.querySelectorAll(
            ":scope > .mis-dashboard-submenu > .mis-dashboard-child"
        );

        if (dashboardChildren.length) {
            return Array.from(dashboardChildren).filter(function (el) {
                const style = window.getComputedStyle(el);
                const rect = el.getBoundingClientRect();
                return (
                    style.display !== "none" &&
                    style.visibility !== "hidden" &&
                    rect.height > 0
                );
            });
        }

        const menu = getDirectMenu(li);
        if (!menu) return [];

        return Array.from(menu.children).filter(function (child) {
            if (child.tagName !== "LI") return false;

            const style = window.getComputedStyle(child);
            const rect = child.getBoundingClientRect();

            return (
                style.display !== "none" &&
                style.visibility !== "hidden" &&
                rect.height > 0
            );
        });
    }

    function getDeepestOpenItem(li) {
        if (!li) return null;

        const children = getVisibleChildren(li);
        if (!children.length) return li;

        /* Prefer an explicitly active child. */
        const activeChild = children.find(function (child) {
            const link = getDirectLink(child);
            return (
                child.classList.contains("active") ||
                !!link?.classList.contains("xon-child-child-active") ||
                !!link?.classList.contains("xon-child-active")
            );
        });

        if (activeChild) {
            return getDeepestOpenItem(activeChild);
        }

        /* Otherwise follow the deepest currently-open branch. */
        for (let i = children.length - 1; i >= 0; i--) {
            const child = children[i];
            if (child.classList.contains("xon-tree-open")) {
                return getDeepestOpenItem(child);
            }
        }

        /* If the parent was just opened, the last visible child is the
           part most likely to be pushed below the viewport. */
        return children[children.length - 1];
    }

    function getDeepestActiveLink(nav) {
        if (!nav) return null;

        return (
            nav.querySelector("a.xon-child-child-active") ||
            nav.querySelector("a.xon-child-active") ||
            nav.querySelector("a.xon-parent-active") ||
            nav.querySelector("a.mis-active") ||
            null
        );
    }

    function getTargetElement(li) {
        if (!li) return null;

        if (li.classList.contains("mis-dashboard-child")) {
            return li;
        }

        return getDirectLink(li) || li;
    }

    function isVisibleEnough(target, container, margin) {
        if (!target || !container) return true;

        const c = container.getBoundingClientRect();
        const r = target.getBoundingClientRect();

        return (
            r.top >= c.top + margin &&
            r.bottom <= c.bottom - margin
        );
    }

    function scrollSidebarToTarget(target, smooth) {
        const container = getScrollContainer();
        if (!container || !target) return;

        const margin = 14;
        const containerRect = container.getBoundingClientRect();
        const targetRect = target.getBoundingClientRect();

        let delta = 0;

        /* Target is below the visible sidebar area. */
        if (targetRect.bottom > containerRect.bottom - margin) {
            delta = targetRect.bottom - (containerRect.bottom - margin);
        }
        /* Target is above the visible sidebar area. */
        else if (targetRect.top < containerRect.top + margin) {
            delta = targetRect.top - (containerRect.top + margin);
        }

        if (Math.abs(delta) < 2) return;

        const maxScroll = Math.max(
            0,
            container.scrollHeight - container.clientHeight
        );

        const nextTop = Math.max(
            0,
            Math.min(
                maxScroll,
                container.scrollTop + delta
            )
        );

        if (Math.abs(nextTop - container.scrollTop) < 1) return;

        container.scrollTo({
            top: nextTop,
            behavior: smooth ? "smooth" : "auto"
        });
    }

    function revealMenu(li, smooth) {
        if (!li) return;

        /*
         * First try the exact clicked/active item. This is important
         * for pages such as:
         * FINANCIAL ACCOUNTING > Bank Payment > Bank Payment Dashboard
         */
        let target = getTargetElement(li);

        /* If this is an opened parent, reveal its deepest visible child. */
        if (
            li.classList.contains("xon-tree-open") ||
            li.classList.contains("xon-tree-active-parent")
        ) {
            const deepest = getDeepestOpenItem(li);
            if (deepest) {
                target = getTargetElement(deepest);
            }
        }

        if (!target) return;

        /* Browser layout can settle one frame later after a submenu opens. */
        requestAnimationFrame(function () {
            scrollSidebarToTarget(target, smooth);
        });
    }

    function revealCurrentBranch() {
        const nav = document.querySelector(
            "#main_nav > .navbar-nav"
        );

        if (!nav) return;

        const activeLink = getDeepestActiveLink(nav);

        if (activeLink) {
            const activeLi = activeLink.closest("li");
            if (activeLi) {
                revealMenu(activeLi, false);
                return;
            }
        }

        const openItems = Array.from(
            nav.querySelectorAll("li.xon-tree-open")
        );

        if (openItems.length) {
            revealMenu(
                openItems[openItems.length - 1],
                false
            );
        }
    }

    function revealAfterLayout(li, smooth) {
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                revealMenu(li, smooth);
            });
        });
    }

    function bind() {
        const nav = document.querySelector(
            "#main_nav > .navbar-nav"
        );

        if (!nav) return false;

        if (nav.dataset[BOUND_FLAG] === "true") {
            return true;
        }

        nav.dataset[BOUND_FLAG] = "true";

        const observer = new MutationObserver(function (mutations) {
            const opened = new Set();

            mutations.forEach(function (mutation) {
                if (
                    mutation.type === "attributes" &&
                    mutation.attributeName === "class" &&
                    mutation.target &&
                    mutation.target.tagName === "LI" &&
                    mutation.target.classList.contains("xon-tree-open")
                ) {
                    opened.add(mutation.target);
                }
            });

            opened.forEach(function (li) {
                revealAfterLayout(li, true);
            });
        });

        observer.observe(nav, {
            subtree: true,
            attributes: true,
            attributeFilter: ["class"]
        });

        nav.addEventListener(
            "shown.bs.dropdown",
            function (e) {
                const li = e.target?.closest
                    ? e.target.closest("li")
                    : null;

                if (li) {
                    revealAfterLayout(li, true);
                }
            },
            false
        );

        nav.addEventListener(
            "click",
            function (e) {
                const link = e.target.closest
                    ? e.target.closest("a, button")
                    : null;

                if (!link || !nav.contains(link)) return;

                const li = link.closest("li");
                if (!li) return;

                revealAfterLayout(li, true);
            },
            false
        );

        function initialisePagePosition() {
            const container = getScrollContainer();
            if (!container) return;

            container.scrollTo({
                top: 0,
                behavior: "auto"
            });

            window.setTimeout(function () {
                revealCurrentBranch();
            }, 120);

            window.setTimeout(function () {
                revealCurrentBranch();
            }, 350);

            window.setTimeout(function () {
                revealCurrentBranch();
            }, 700);
        }

        window.addEventListener(
            "pageshow",
            function () {
                window.setTimeout(initialisePagePosition, 0);
            }
        );

        window.setTimeout(initialisePagePosition, 0);
        window.setTimeout(revealCurrentBranch, 500);
        window.setTimeout(revealCurrentBranch, 1000);

        return true;
    }

    function init() {
        if (bind()) return;
        window.setTimeout(init, 100);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();

(function () {
    function installXonTreeStyle() {
        if (document.getElementById("xon-final-tree-js-style")) return;

        const style = document.createElement("style");
        style.id = "xon-final-tree-js-style";
        style.textContent = `
            #main_nav li.xon-tree-parent > ul.dropdown-menu,
            #main_nav li.xon-tree-parent > ul.submenu {
                position: static !important;
                float: none !important;
                transform: none !important;
                width: 100% !important;
                min-width: 0 !important;
                margin: 2px 0 4px !important;
                padding: 3px 0 3px 18px !important;
                border: 0 !important;
                box-shadow: none !important;
                background: transparent !important;
                overflow: hidden !important;
            }

            #main_nav li.xon-tree-parent > ul.dropdown-menu:not(.xon-tree-open),
            #main_nav li.xon-tree-parent > ul.submenu:not(.xon-tree-open) {
                display: none !important;
                visibility: hidden !important;
                opacity: 0 !important;
                max-height: 0 !important;
            }

            #main_nav li.xon-tree-parent > ul.dropdown-menu.xon-tree-open,
            #main_nav li.xon-tree-parent > ul.submenu.xon-tree-open {
                display: block !important;
                visibility: visible !important;
                opacity: 1 !important;
                max-height: 10000px !important;
            }

            #main_nav li.xon-tree-parent > a .xon-tree-arrow {
                margin-left: auto !important;
                pointer-events: none !important;
                font-size: 11px !important;
                line-height: 1 !important;
            }
        `;
        document.head.appendChild(style);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", installXonTreeStyle);
    } else {
        installXonTreeStyle();
    }
})();

/* =========================================================
   XON ERP - BREADCRUMB PAGE LOAD / MVC NAVIGATION FIX
   ---------------------------------------------------------
   Purpose:
   - Keep #xonBreadcrumb visible after a full MVC page load.
   - Re-detect the current sidebar menu after DB menu rendering.
   - Restore the last sidebar branch for internal pages such as
     Add/Edit/View pages which are not separate menu items.
   - Retry after delayed menu rendering without changing the
     existing sidebar/favourite/dashboard behaviour.
   ========================================================= */
(function () {
    "use strict";

    const BRANCH_KEY = "__XON_ERP_OPEN_BRANCH__";
    let syncTimer = null;
    let observerStarted = false;
    let syncing = false;

    function getNav() {
        return document.querySelector("#main_nav > .navbar-nav");
    }

    function getBreadcrumb() {
        return document.getElementById("xonBreadcrumb");
    }

    function normalizePath(path) {
        return (path || "/")
            .toLowerCase()
            .replace(/\/+$/, "") || "/";
    }

    function getDirectLink(li) {
        if (!li) return null;

        return li.querySelector(
            ":scope > a.nav-link, " +
            ":scope > a.dropdown-item, " +
            ":scope > a"
        );
    }

    function stableId(link) {
        if (!link) return null;

        const menuKey =
            link.getAttribute("data-menu-key") ||
            link.getAttribute("data-menukey") ||
            link.dataset.menuKey;

        if (menuKey && menuKey.toString().trim()) {
            return {
                type: "key",
                value: menuKey.toString().trim()
            };
        }

        const href =
            (link.getAttribute("href") || "").trim();

        if (
            href &&
            href !== "#" &&
            !href.toLowerCase().startsWith("javascript:")
        ) {
            try {
                const u = new URL(href, window.location.origin);
                return {
                    type: "path",
                    value: normalizePath(u.pathname)
                };
            } catch (_) { }
        }

        const text =
            (link.textContent || "")
                .replace(/\s+/g, " ")
                .trim();

        return text
            ? { type: "text", value: text.toLowerCase() }
            : null;
    }

    function sameStableId(link, saved) {
        if (!link || !saved) return false;

        const current = stableId(link);
        if (!current) return false;

        if (
            current.type === saved.type &&
            current.value === saved.value
        ) {
            return true;
        }

        /* Ignore query-string/formKey changes. */
        if (saved.type === "path") {
            const href =
                (link.getAttribute("href") || "").trim();

            if (href) {
                try {
                    const u = new URL(href, window.location.origin);
                    return normalizePath(u.pathname) === saved.value;
                } catch (_) { }
            }
        }

        return false;
    }

    function hasSubMenu(li) {
        return !!(
            li &&
            li.querySelector(
                ":scope > ul.dropdown-menu, :scope > ul.submenu"
            )
        );
    }

    function openTreeItem(li) {
        if (!li) return;

        li.classList.add("xon-tree-open", "xon-tree-active-parent");

        const menu = li.querySelector(
            ":scope > ul.dropdown-menu, :scope > ul.submenu"
        );

        if (menu) {
            menu.classList.add("xon-tree-open");
            menu.classList.remove("show");
            menu.setAttribute("aria-hidden", "false");
        }

        const arrow = li.querySelector(":scope > a > .xon-tree-arrow");
        if (arrow) arrow.textContent = "−";
    }

    function clearXonActiveClasses(nav) {
        nav.querySelectorAll(
            "a.xon-parent-active, " +
            "a.xon-child-active, " +
            "a.xon-child-child-active"
        ).forEach(function (link) {
            link.classList.remove(
                "xon-parent-active",
                "xon-child-active",
                "xon-child-child-active"
            );
        });
    }

    function getChain(link, nav) {
        const chain = [];
        let li = link && link.closest("li");

        while (li && nav.contains(li)) {
            chain.unshift(li);

            if (li.parentElement === nav) {
                break;
            }

            li =
                li.parentElement &&
                li.parentElement.closest("li");
        }

        return chain;
    }

    function applyActiveChain(link, nav) {
        if (!link || !nav) return false;

        const chain = getChain(link, nav);
        if (!chain.length) return false;

        clearXonActiveClasses(nav);

        chain.forEach(function (li, index) {
            const directLink = getDirectLink(li);
            if (!directLink) return;

            directLink.classList.add("active");

            if (index === 0) {
                directLink.classList.add("xon-parent-active");
            } else if (
                index === chain.length - 1 &&
                index >= 2
            ) {
                directLink.classList.add("xon-child-child-active");
            } else {
                directLink.classList.add("xon-child-active");
            }

            if (index < chain.length - 1 || hasSubMenu(li)) {
                openTreeItem(li);
            }
        });

        return true;
    }

    function findCurrentMenuLink(nav) {
        const currentPath = normalizePath(window.location.pathname);
        let result = null;

        nav.querySelectorAll("a[href]").forEach(function (link) {
            if (result) return;

            const href =
                (link.getAttribute("href") || "").trim();

            if (
                !href ||
                href === "#" ||
                href.toLowerCase().startsWith("javascript:")
            ) {
                return;
            }

            try {
                const url = new URL(href, window.location.origin);
                if (normalizePath(url.pathname) === currentPath) {
                    result = link;
                }
            } catch (_) { }
        });

        return result;
    }

    function readSavedBranch() {
        try {
            const raw = sessionStorage.getItem(BRANCH_KEY);
            if (!raw) return null;

            const data = JSON.parse(raw);
            if (
                !data ||
                !Array.isArray(data.chain) ||
                !data.chain.length
            ) {
                return null;
            }

            return data;
        } catch (_) {
            return null;
        }
    }

    function findSavedChild(container, savedId, nav) {
        if (!container || !savedId) return null;

        let items;

        if (container === nav) {
            items = Array.from(nav.children).filter(function (el) {
                return el.tagName === "LI";
            });
        } else {
            const menu = container.querySelector(
                ":scope > ul.dropdown-menu, :scope > ul.submenu"
            );

            items = menu
                ? Array.from(menu.children).filter(function (el) {
                    return el.tagName === "LI";
                })
                : [];
        }

        /* DB-generated wrappers fallback. */
        if (!items.length) {
            items = Array.from(container.querySelectorAll("li")).filter(
                function (li) {
                    return (
                        li.parentElement &&
                        li.parentElement.closest("li") === container
                    );
                }
            );
        }

        for (let i = 0; i < items.length; i++) {
            const link = getDirectLink(items[i]);
            if (link && sameStableId(link, savedId)) {
                return items[i];
            }
        }

        return null;
    }

    function restoreSavedBranch(nav) {
        const saved = readSavedBranch();
        if (!saved) return false;

        let container = nav;
        const resolved = [];

        for (let i = 0; i < saved.chain.length; i++) {
            const li = findSavedChild(
                container,
                saved.chain[i],
                nav
            );

            if (!li) break;

            resolved.push(li);
            container = li;
        }

        if (!resolved.length) return false;

        clearXonActiveClasses(nav);

        resolved.forEach(function (li, index) {
            const link = getDirectLink(li);
            if (!link) return;

            link.classList.add("active");

            if (index === 0) {
                link.classList.add("xon-parent-active");
            } else if (
                index === resolved.length - 1 &&
                index >= 2
            ) {
                link.classList.add("xon-child-child-active");
            } else {
                link.classList.add("xon-child-active");
            }

            if (index < resolved.length - 1 || hasSubMenu(li)) {
                openTreeItem(li);
            }
        });

        return true;
    }

    function breadcrumbHasContent() {
        const breadcrumb = getBreadcrumb();
        if (!breadcrumb) return false;

        return !!(
            breadcrumb.textContent &&
            breadcrumb.textContent.replace(/\s+/g, " ").trim() &&
            breadcrumb.classList.contains("is-visible")
        );
    }

    function updateBreadcrumb() {
        if (typeof window.xonUpdateBreadcrumb !== "function") {
            return;
        }

        window.xonUpdateBreadcrumb();
    }

    function syncNow() {
        if (syncing) return;
        syncing = true;

        try {
            const nav = getNav();
            const breadcrumb = getBreadcrumb();

            if (!nav || !breadcrumb) {
                return;
            }

            /* If another part of the page has already produced a valid
               breadcrumb, do not disturb it. */
            if (breadcrumbHasContent()) {
                return;
            }

            /* First preference: exact current MVC route. */
            const currentLink = findCurrentMenuLink(nav);
            if (currentLink) {
                applyActiveChain(currentLink, nav);
                updateBreadcrumb();
                return;
            }

            /* Second preference: the branch saved before navigation.
               This is required for Add/Edit/View pages which do not have
               their own sidebar URL. */
            if (restoreSavedBranch(nav)) {
                updateBreadcrumb();
            }
        } finally {
            syncing = false;
        }
    }

    function scheduleSync(delay) {
        window.setTimeout(syncNow, delay || 0);
    }

    function initObserver() {
        const nav = getNav();
        if (!nav || observerStarted) return;

        observerStarted = true;

        const observer = new MutationObserver(function () {
            const breadcrumb = getBreadcrumb();

            /* Only repair when breadcrumb is missing/empty. This prevents
               an observer loop when xonUpdateBreadcrumb() rebuilds it. */
            if (!breadcrumbHasContent() && breadcrumb) {
                if (syncTimer) {
                    window.clearTimeout(syncTimer);
                }

                syncTimer = window.setTimeout(function () {
                    syncNow();
                }, 120);
            }
        });

        observer.observe(nav, {
            childList: true,
            subtree: true,
            attributes: true,
            attributeFilter: ["class", "href", "data-menu-key", "data-menukey"]
        });
    }

    function initPageLoadBreadcrumbFix() {
        initObserver();

        /* Menu/sidebar rendering can happen at different times on MVC
           pages, so intentionally retry at several safe points. */
        [50, 200, 450, 800, 1200, 1800, 2500].forEach(function (delay) {
            scheduleSync(delay);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            initPageLoadBreadcrumbFix
        );
    } else {
        initPageLoadBreadcrumbFix();
    }

    window.addEventListener("load", function () {
        [100, 500, 1000].forEach(function (delay) {
            scheduleSync(delay);
        });
    });

    window.addEventListener("pageshow", function () {
        [100, 500, 1000].forEach(function (delay) {
            scheduleSync(delay);
        });
    });


    /* ==========================================================
       FULL DATABASE MENU NAME ON HOVER
       Visible label remains compact/ellipsis.
       Custom tooltip appears immediately on hover.
       ========================================================== */
    (function setupFullMenuNameHover() {
        const TOOLTIP_CLASS = "xon-menu-hover-tooltip";

        function installTooltipStyle() {
            if (document.getElementById("xon-instant-menu-tooltip-style")) {
                return;
            }

            const style = document.createElement("style");
            style.id = "xon-instant-menu-tooltip-style";
            style.textContent = `
                .xon-menu-hover-tooltip {
                    position: fixed !important;
                    z-index: 999999 !important;
                    display: block !important;
                    max-width: 320px !important;
                    padding: 6px 10px !important;
                    background: #ffffff !important;
                    color: #000000 !important;
                    border: 1px solid #b8b8b8 !important;
                    border-radius: 4px !important;
                    font-family: Arial, Helvetica, sans-serif !important;
                    font-size: 12px !important;
                    font-weight: 700 !important;
                    line-height: 1.3 !important;
                    white-space: nowrap !important;
                    pointer-events: none !important;
                    box-shadow: 0 3px 10px rgba(0,0,0,.20) !important;
                }
            `;
            document.head.appendChild(style);
        }

        function removeTooltip() {
            document.querySelectorAll("." + TOOLTIP_CLASS).forEach(function (tooltip) {
                tooltip.remove();
            });
        }

        function applyFullNames(root) {
            const scope = root && root.querySelectorAll ? root : document;

            scope.querySelectorAll("#main_nav .mis-label").forEach(function (label) {
                const fullName = (label.textContent || "")
                    .replace(/\s+/g, " ")
                    .trim();

                if (!fullName) return;

                // Disable native browser tooltip because it has a visible delay.
                label.removeAttribute("title");

                const link = label.closest("a");
                if (link) {
                    link.removeAttribute("title");

                    // IMPORTANT: never replace DB Mainmenuheading with the
                    // visible DisplayMainMenuHeading/compact label.
                    const dbMainMenuHeading =
                        (link.getAttribute("data-main-menu-heading") || "").trim();

                    if (dbMainMenuHeading) {
                        link.setAttribute(
                            "data-full-menu-name",
                            dbMainMenuHeading
                        );
                    }
                }
            });
        }

        function showTooltip(label) {
            const link = label.closest("a");
            if (!link) return;

            // Hover must show DB Mainmenuheading, NOT the visible label.
            const fullName =
                (link.getAttribute("data-main-menu-heading") || "").trim();

            if (!fullName) return;

            removeTooltip();

            const tooltip = document.createElement("div");
            tooltip.className = TOOLTIP_CLASS;
            tooltip.textContent = fullName;
            document.body.appendChild(tooltip);

            const rect = link.getBoundingClientRect();

            let top = rect.top + rect.height + 3;
            let left = rect.left;

            const tooltipWidth = tooltip.offsetWidth;
            const tooltipHeight = tooltip.offsetHeight;

            if (left + tooltipWidth > window.innerWidth - 10) {
                left = Math.max(10, window.innerWidth - tooltipWidth - 10);
            }

            if (top + tooltipHeight > window.innerHeight - 10) {
                top = rect.top - tooltipHeight - 3;
            }

            tooltip.style.top = Math.max(5, top) + "px";
            tooltip.style.left = Math.max(5, left) + "px";
        }

        function init() {
            installTooltipStyle();
            applyFullNames(document);

            const nav = document.querySelector("#main_nav");
            if (!nav || nav.dataset.xonFullNameHoverBound === "true") {
                return;
            }

            nav.dataset.xonFullNameHoverBound = "true";

            // mouseover/mouseout are delegated so dynamically-created DB menus work too.
            nav.addEventListener("mouseover", function (e) {
                const label = e.target && e.target.closest
                    ? e.target.closest(".mis-label")
                    : null;

                if (!label || !nav.contains(label)) return;

                const fromElement = e.relatedTarget;
                if (fromElement && label.contains(fromElement)) return;

                applyFullNames(label.parentElement || label);
                showTooltip(label);
            }, true);

            nav.addEventListener("mouseout", function (e) {
                const label = e.target && e.target.closest
                    ? e.target.closest(".mis-label")
                    : null;

                if (!label) return;

                const toElement = e.relatedTarget;
                if (toElement && label.contains(toElement)) return;

                removeTooltip();
            }, true);

            const observer = new MutationObserver(function (mutations) {
                mutations.forEach(function (mutation) {
                    mutation.addedNodes.forEach(function (node) {
                        if (node.nodeType === 1) {
                            applyFullNames(node);
                        }
                    });
                });
            });

            observer.observe(nav, {
                childList: true,
                subtree: true
            });

            window.addEventListener("scroll", removeTooltip, true);
            window.addEventListener("resize", removeTooltip);
        }

        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", init);
        } else {
            init();
        }
    })();
})();

/* =========================================================
   XON ERP - FINAL OPEN SIDEBAR STATE PERSISTENCE FIX
   ---------------------------------------------------------
   Keeps the branch that the user opened expanded after MVC
   page navigation / refresh.

   IMPORTANT:
   - Saves AFTER the existing tree click handler has finished.
   - Stores ALL currently-open branches, not only the active leaf.
   - Restores only the saved open branches.
   - Does not interfere with favourites or breadcrumb logic.
   ========================================================= */
(function () {
    "use strict";

    const OPEN_STATE_KEY = "__XON_ERP_OPEN_SIDEBAR_STATE_V2__";

    function getNav() {
        return document.querySelector("#main_nav > .navbar-nav");
    }

    function getDirectLink(li) {
        if (!li) return null;
        return li.querySelector(
            ":scope > a.nav-link, :scope > a.dropdown-item, :scope > a"
        );
    }

    function getStableId(link) {
        if (!link) return null;

        const menuKey =
            link.getAttribute("data-menu-key") ||
            link.getAttribute("data-menukey") ||
            link.dataset.menuKey;

        if (menuKey && menuKey.toString().trim()) {
            return {
                type: "key",
                value: menuKey.toString().trim()
            };
        }

        const href = (link.getAttribute("href") || "").trim();
        if (href && href !== "#" && !href.toLowerCase().startsWith("javascript:")) {
            try {
                const url = new URL(href, window.location.origin);
                return {
                    type: "path",
                    value: (url.pathname || "/")
                        .toLowerCase()
                        .replace(/\/+$/, "") || "/"
                };
            } catch (_) { }
        }

        const dashboard = link.getAttribute("data-dashboard-tab");
        if (dashboard) {
            return {
                type: "dashboard",
                value: dashboard.toString().trim()
            };
        }

        const text = (link.textContent || "")
            .replace(/\s+/g, " ")
            .trim();

        return text
            ? { type: "text", value: text.toLowerCase() }
            : null;
    }

    function sameId(link, saved) {
        const current = getStableId(link);
        if (!current || !saved) return false;

        if (current.type === saved.type && current.value === saved.value) {
            return true;
        }

        /* A formKey/query-string change must not break path matching. */
        if (saved.type === "path") {
            const href = (link.getAttribute("href") || "").trim();
            if (href) {
                try {
                    const url = new URL(href, window.location.origin);
                    const path = (url.pathname || "/")
                        .toLowerCase()
                        .replace(/\/+$/, "") || "/";
                    return path === saved.value;
                } catch (_) { }
            }
        }

        return false;
    }

    function directMenu(li) {
        if (!li) return null;
        return li.querySelector(
            ":scope > ul.dropdown-menu, :scope > ul.submenu"
        );
    }

    function hasMenu(li) {
        return !!directMenu(li);
    }

    function getChildren(container) {
        if (!container) return [];

        if (container.tagName === "UL") {
            return Array.from(container.children).filter(function (el) {
                return el.tagName === "LI";
            });
        }

        const menu = directMenu(container);
        if (menu) {
            return Array.from(menu.children).filter(function (el) {
                return el.tagName === "LI";
            });
        }

        return [];
    }

    function findChild(container, savedId) {
        const items = getChildren(container);

        for (let i = 0; i < items.length; i++) {
            const link = getDirectLink(items[i]);
            if (link && sameId(link, savedId)) {
                return items[i];
            }
        }

        return null;
    }

    function setOpen(li, open) {
        if (!li) return;

        const menu = directMenu(li);
        if (!menu) return;

        li.classList.toggle("xon-tree-open", open);
        li.classList.toggle("xon-tree-click-active", open);

        menu.classList.toggle("xon-tree-open", open);
        menu.classList.remove("show");
        menu.setAttribute("aria-hidden", open ? "false" : "true");

        const arrow = li.querySelector(":scope > a > .xon-tree-arrow");
        if (arrow) {
            arrow.textContent = open ? "−" : "+";
        }
    }

    function getChainForLi(li, nav) {
        const chain = [];
        let current = li;

        while (current && nav.contains(current)) {
            const link = getDirectLink(current);
            if (link) {
                const id = getStableId(link);
                if (id) chain.unshift(id);
            }

            if (current.parentElement === nav) break;

            current = current.parentElement
                ? current.parentElement.closest("li")
                : null;
        }

        return chain;
    }

    function saveOpenState() {
        const nav = getNav();
        if (!nav) return;

        const openBranches = [];

        nav.querySelectorAll("li.xon-tree-open").forEach(function (li) {
            if (!hasMenu(li)) return;

            const chain = getChainForLi(li, nav);
            if (chain.length) {
                openBranches.push(chain);
            }
        });

        /* Remove duplicate chains. */
        const unique = [];
        const seen = new Set();

        openBranches.forEach(function (chain) {
            const key = JSON.stringify(chain);
            if (!seen.has(key)) {
                seen.add(key);
                unique.push(chain);
            }
        });

        try {
            sessionStorage.setItem(
                OPEN_STATE_KEY,
                JSON.stringify({
                    branches: unique,
                    savedAt: Date.now(),
                    path: window.location.pathname
                })
            );
        } catch (_) { }
    }

    function readOpenState() {
        try {
            const raw = sessionStorage.getItem(OPEN_STATE_KEY);
            if (!raw) return null;

            const data = JSON.parse(raw);
            if (!data || !Array.isArray(data.branches)) return null;

            return data;
        } catch (_) {
            return null;
        }
    }

    function restoreOpenState() {
        const nav = getNav();
        const saved = readOpenState();

        if (!nav || !saved || !saved.branches.length) return false;

        let restored = false;

        saved.branches.forEach(function (chain) {
            if (!Array.isArray(chain) || !chain.length) return;

            let container = nav;

            for (let i = 0; i < chain.length; i++) {
                const li = findChild(container, chain[i]);
                if (!li) return;

                if (hasMenu(li)) {
                    setOpen(li, true);
                    li.classList.add("xon-tree-active-parent");
                    restored = true;
                }

                container = li;
            }
        });

        if (restored && typeof window.xonUpdateBreadcrumb === "function") {
            window.setTimeout(function () {
                window.xonUpdateBreadcrumb();
            }, 30);
        }

        return restored;
    }

    function scheduleSave() {
        /* Existing click handler runs before this timer. */
        window.setTimeout(saveOpenState, 0);
        window.setTimeout(saveOpenState, 80);
    }

    function bind() {
        const nav = getNav();
        if (!nav) return;

        if (document.documentElement.dataset.xonOpenStateV2Bound !== "true") {
            document.documentElement.dataset.xonOpenStateV2Bound = "true";

            /* Capture the click, then save after the existing handler changes
               xon-tree-open / xon-tree-active-parent classes. */
            nav.addEventListener("click", function () {
                scheduleSave();
            }, true);

            /* Also save immediately before leaving the current document. */
            window.addEventListener("beforeunload", saveOpenState);
            window.addEventListener("pagehide", saveOpenState);
        }

        /* Restore after the existing initTree() has finished closing menus. */
        [150, 400, 800, 1300, 2000].forEach(function (delay) {
            window.setTimeout(restoreOpenState, delay);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", bind);
    } else {
        bind();
    }

    window.addEventListener("load", function () {
        [150, 500, 1000].forEach(function (delay) {
            window.setTimeout(restoreOpenState, delay);
        });
    });

    window.addEventListener("pageshow", function () {
        [150, 500, 1000].forEach(function (delay) {
            window.setTimeout(restoreOpenState, delay);
        });
    });
})();