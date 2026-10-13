
(function () {
    var sidebar = document.getElementById("sidebar");
    var collapseButton = document.getElementById("collapseButton");
    var tooltip = document.getElementById("milestoneTooltip");
    var addClientButton = document.getElementById("addClientButton");
    var emptyAddButton = document.getElementById("emptyAddButton");

    if (!sidebar) {
        return;
    }

    var savedState = localStorage.getItem("bouqetPlannerSidebar");

    if (savedState === "expanded") {
        sidebar.classList.add("expanded");
    }

    if (collapseButton) {
        collapseButton.addEventListener("click", function () {
            sidebar.classList.toggle("expanded");

            localStorage.setItem(
                "bouqetPlannerSidebar",
                sidebar.classList.contains("expanded")
                    ? "expanded"
                    : "collapsed"
            );
        });
    }

    var navItems = document.querySelectorAll(".nav-item");

    for (var i = 0; i < navItems.length; i++) {
        navItems[i].addEventListener("mouseenter", function () {
            if (!sidebar.classList.contains("expanded")) {
                this.setAttribute(
                    "data-tooltip",
                    this.getAttribute("title") || ""
                );
            }
        });

        navItems[i].addEventListener("click", function (event) {
            var target = this.getAttribute("data-section");

            if (!target) {
                return;
            }

            event.preventDefault();

            scrollToSection(target);
        });
    }

    var sections = document.querySelectorAll(".dashboard-section");

    function scrollToSection(sectionId) {
        var section = document.getElementById(sectionId);

        if (!section) {
            return;
        }

        section.scrollIntoView({
            behavior: "smooth",
            block: "start"
        });

        setActiveSection(sectionId);
    }

    function setActiveSection(sectionId) {
        for (var i = 0; i < navItems.length; i++) {
            var itemSection = navItems[i].getAttribute("data-section");

            if (itemSection === sectionId) {
                navItems[i].classList.add("active");
            } else {
                navItems[i].classList.remove("active");
            }
        }
    }

    function updateActiveSection() {
        var currentSection = "dashboard";
        var closestDistance = Infinity;

        for (var i = 0; i < sections.length; i++) {
            var rect = sections[i].getBoundingClientRect();
            var distance = Math.abs(rect.top - 130);

            if (rect.top <= 180 && distance < closestDistance) {
                closestDistance = distance;
                currentSection = sections[i].id;
            }
        }

        setActiveSection(currentSection);
    }

    window.addEventListener("scroll", function () {
        updateActiveSection();
    }, {
        passive: true
    });

    var milestones = document.querySelectorAll(".milestone-dot");

    for (var j = 0; j < milestones.length; j++) {
        milestones[j].addEventListener("mouseenter", function (event) {
            showTooltip(this, event);
        });

        milestones[j].addEventListener("mousemove", function (event) {
            moveTooltip(event);
        });

        milestones[j].addEventListener("mouseleave", function () {
            hideTooltip();
        });

        milestones[j].addEventListener("click", function () {
            scrollToSection("tasks");
        });
    }

    var weddingButtons =
        document.querySelectorAll(".lane-label, .wedding-day");

    for (var k = 0; k < weddingButtons.length; k++) {
        weddingButtons[k].addEventListener("click", function () {
            scrollToSection("weddings");
        });
    }

    var requestButtons =
        document.querySelectorAll(".pending-request");

    for (var r = 0; r < requestButtons.length; r++) {
        requestButtons[r].addEventListener("click", function () {
            scrollToSection("requests");
        });
    }

    if (addClientButton) {
        addClientButton.addEventListener("click", function () {
            window.location.href =
                "24-1528LibrandaMidPlannerAddClient.aspx";
        });
    }

    if (emptyAddButton) {
        emptyAddButton.addEventListener("click", function () {
            window.location.href =
                "24-1528LibrandaMidPlannerAddClient.aspx";
        });
    }

    function showTooltip(element, event) {
        if (!tooltip) {
            return;
        }

        var title =
            element.getAttribute("data-title") || "";

        var type =
            element.getAttribute("data-type") || "";

        var date =
            element.getAttribute("data-date") || "";

        tooltip.innerHTML =
            "<div class=\"tooltip-title\">" +
            escapeHtml(title) +
            "</div>" +
            "<div class=\"tooltip-meta\">" +
            escapeHtml(type) +
            " · " +
            escapeHtml(date) +
            "</div>";

        tooltip.classList.add("visible");

        moveTooltip(event);
    }

    function moveTooltip(event) {
        if (!tooltip ||
            !tooltip.classList.contains("visible")) {
            return;
        }

        var left = event.clientX + 14;
        var top = event.clientY + 14;

        var width = tooltip.offsetWidth;
        var height = tooltip.offsetHeight;

        if (left + width > window.innerWidth - 12) {
            left = event.clientX - width - 14;
        }

        if (top + height > window.innerHeight - 12) {
            top = event.clientY - height - 14;
        }

        tooltip.style.left = left + "px";
        tooltip.style.top = top + "px";
    }

    function hideTooltip() {
        if (!tooltip) {
            return;
        }

        tooltip.classList.remove("visible");
    }

    function escapeHtml(value) {
        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    if (window.location.hash) {
        var initialSection =
            window.location.hash.substring(1);

        setTimeout(function () {
            scrollToSection(initialSection);
        }, 100);
    } else {
        setActiveSection("dashboard");
    }
})();
