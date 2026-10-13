(function () {
    var sidebar = document.getElementById("sidebar");
    var collapseButton = document.getElementById("collapseButton");

    if (!sidebar || !collapseButton) {
        return;
    }

    var storageKey = "bouqetPlannerSidebarOpen";

    function setSidebarState(open) {
        if (open) {
            sidebar.classList.remove("collapsed");
            localStorage.setItem(storageKey, "true");
        } else {
            sidebar.classList.add("collapsed");
            localStorage.setItem(storageKey, "false");
        }
    }

    collapseButton.addEventListener("click", function () {
        setSidebarState(sidebar.classList.contains("collapsed"));
    });

    var savedState = localStorage.getItem(storageKey);

    if (savedState === "false") {
        sidebar.classList.add("collapsed");
    }
})();