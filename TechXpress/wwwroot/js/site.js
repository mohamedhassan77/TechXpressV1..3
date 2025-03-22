document.addEventListener("DOMContentLoaded", () => {
    // ----- Mobile Menu Toggle -----
    const mobileToggle = document.querySelector(".mobile-menu-toggle");
    const navMenu = document.querySelector(".nav-menu");
    if (mobileToggle && navMenu) {
        mobileToggle.addEventListener("click", function () {
            const expanded = this.getAttribute("aria-expanded") === "true";
            this.setAttribute("aria-expanded", !expanded);
            navMenu.classList.toggle("active");
        });
    }

    // ----- Cart and Wishlist Counts -----
    async function updateCartAndWishlistCounts() {
        try {
            const cartResponse = await fetch('/Cart/GetCartCount');
            if (cartResponse.ok) {
                const cartData = await cartResponse.json();
                const cartCounter = document.getElementById("cart-counter");
                if (cartCounter) cartCounter.innerText = cartData.count;
            } else {
                console.error("Failed to fetch cart count");
            }

            const wishlistResponse = await fetch('/Wishlist/GetWishlistCount');
            if (wishlistResponse.ok) {
                const wishlistData = await wishlistResponse.json();
                const wishlistCounter = document.getElementById("wishlist-counter");
                if (wishlistCounter) wishlistCounter.innerText = wishlistData.count;
            } else {
                console.error("Failed to fetch wishlist count");
            }
        } catch (error) {
            console.error("Error updating counts:", error);
        }
    }
    updateCartAndWishlistCounts();
    setInterval(updateCartAndWishlistCounts, 10000);

    // ----- Profile Dropdown Toggle -----
    const profileTrigger = document.getElementById("profileTrigger");
    const profileDropdown = document.getElementById("profileDropdown");
    if (profileTrigger && profileDropdown) {
        profileTrigger.addEventListener("click", (e) => {
            e.preventDefault();
            profileDropdown.classList.toggle("show");
        });
        document.addEventListener("click", (e) => {
            if (!profileTrigger.contains(e.target) && !profileDropdown.contains(e.target)) {
                profileDropdown.classList.remove("show");
            }
        });
    }

    // ----- Smooth Scrolling for Anchor Links -----
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener("click", (e) => {
            e.preventDefault();
            const targetId = anchor.getAttribute("href");
            if (targetId && targetId !== "#") {
                const target = document.querySelector(targetId);
                if (target) target.scrollIntoView({ behavior: "smooth", block: "start" });
            }
        });
    });

    // ----- Category Dropdown Toggle -----
    const catTrigger = document.getElementById("categoryTrigger");
    const catDropdown = document.getElementById("categoryDropdown");
    if (catTrigger && catDropdown) {
        catTrigger.addEventListener("click", function (e) {
            e.preventDefault();
            const isExpanded = catDropdown.classList.toggle("show");
            catTrigger.setAttribute("aria-expanded", isExpanded);
        });
        document.addEventListener("click", function (e) {
            if (!catTrigger.contains(e.target) && !catDropdown.contains(e.target)) {
                catDropdown.classList.remove("show");
                catTrigger.setAttribute("aria-expanded", "false");
            }
        });
    }

    // ----- Continuous Scrolling for Category Dropdown -----
    let scrollInterval;
    const startScrolling = (direction) => {
        stopScrolling();
        scrollInterval = setInterval(() => {
            catDropdown.scrollBy({ left: direction * 50, behavior: 'auto' });
        }, 20);
    };
    const stopScrolling = () => clearInterval(scrollInterval);
    if (catDropdown) {
        catDropdown.addEventListener('mouseover', (e) => {
            const rect = catDropdown.getBoundingClientRect();
            const mouseX = e.clientX - rect.left;
            if (mouseX < 50) startScrolling(-1);
            else if (mouseX > rect.width - 50) startScrolling(1);
        });
        catDropdown.addEventListener('mousemove', (e) => {
            const rect = catDropdown.getBoundingClientRect();
            const mouseX = e.clientX - rect.left;
            if (mouseX < 50) startScrolling(-1);
            else if (mouseX > rect.width - 50) startScrolling(1);
            else stopScrolling();
        });
        catDropdown.addEventListener('mouseleave', stopScrolling);
    }

    
});