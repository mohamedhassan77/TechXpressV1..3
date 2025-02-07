document.addEventListener("DOMContentLoaded", function () {
    const menuToggle = document.getElementById("menu-toggle");
    const navMenu = document.querySelector(".nav-menu");

    menuToggle.addEventListener("click", function () {
        navMenu.classList.toggle("active");
    });
});

    document.addEventListener("DOMContentLoaded", function () {
    const profileTrigger = document.getElementById("profileTrigger");
    const profileDropdown = document.getElementById("profileDropdown");

    if (profileTrigger && profileDropdown) {
        profileTrigger.addEventListener("click", function (event) {
            event.preventDefault();
            profileDropdown.classList.toggle("show");
        });

    document.addEventListener("click", function (event) {
            if (!profileTrigger.contains(event.target) && !profileDropdown.contains(event.target)) {
        profileDropdown.classList.remove("show");
            }
        });
    }
});

