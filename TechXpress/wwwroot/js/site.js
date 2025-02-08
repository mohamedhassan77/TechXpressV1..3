document.addEventListener("DOMContentLoaded", function () {
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

    // ----- Profile Dropdown Toggle -----
    const profileTrigger = document.getElementById("profileTrigger");
    const profileDropdown = document.getElementById("profileDropdown");
    if (profileTrigger && profileDropdown) {
        profileTrigger.addEventListener("click", function (e) {
            e.preventDefault();
            profileDropdown.classList.toggle("show");
        });
        document.addEventListener("click", function (e) {
            if (!profileTrigger.contains(e.target) && !profileDropdown.contains(e.target)) {
                profileDropdown.classList.remove("show");
            }
        });
    }

    // ----- Categories Dropdown Toggle (Two-Column) -----
    // Categories Dropdown Toggle
    const categoriesDropdownTrigger = document.getElementById("categoriesDropdown");
    const twoColumnDropdown = document.querySelector(".two-column-dropdown");
    if (categoriesDropdownTrigger && twoColumnDropdown) {
        categoriesDropdownTrigger.addEventListener("click", function (e) {
            e.preventDefault();
            twoColumnDropdown.classList.toggle("show");
            const expanded = categoriesDropdownTrigger.getAttribute("aria-expanded") === "true";
            categoriesDropdownTrigger.setAttribute("aria-expanded", !expanded);
        });
        document.addEventListener("click", function (e) {
            if (!e.target.closest(".nav-item.dropdown")) {
                twoColumnDropdown.classList.remove("show");
                categoriesDropdownTrigger.setAttribute("aria-expanded", "false");
            }
        });
    }

    // ----- Smooth Scrolling for Anchor Links -----
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener("click", function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute("href"));
            if (target) {
                target.scrollIntoView({ behavior: "smooth", block: "start" });
            }
        });
    });

    // ----- Modal Functionality -----
    window.openModal = function (name, description, price, imageUrl, productId) {
        const modal = document.getElementById("productModal");
        modal.style.display = "flex";
        setTimeout(() => { modal.classList.add("active"); }, 10);
        document.getElementById("modal-product-name").innerText = name;
        document.getElementById("modal-product-description").innerText = description;
        document.getElementById("modal-product-price").innerText = parseFloat(price).toLocaleString("en-US", {
            style: "currency",
            currency: "EGP"
        });
        document.getElementById("modal-product-image").src = imageUrl;
        document.getElementById("modal-thumbnail-1").src = imageUrl;
        // Set hidden fields with productId
        document.getElementById("modalProductId").value = productId;
        document.getElementById("modalProductIdOrder").value = productId;
    };

    window.closeModal = function () {
        const modal = document.getElementById("productModal");
        modal.classList.remove("active");
        setTimeout(() => { modal.style.display = "none"; }, 300);
    };

    window.updateQuantity = function (change) {
        const quantityInput = document.getElementById("quantity");
        let currentValue = parseInt(quantityInput.value);
        let newValue = currentValue + change;
        newValue = Math.max(1, Math.min(newValue, 99));
        quantityInput.value = newValue;
    };

    const modalElement = document.getElementById("productModal");
    if (modalElement) {
        modalElement.addEventListener("click", function (e) {
            if (e.target === modalElement) {
                window.closeModal();
            }
        });
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape" && modalElement.style.display === "flex") {
                window.closeModal();
            }
        });
    }

    // ----- Update Cart and Wishlist Counters -----
    async function updateCounters() {
        try {
            const cartResponse = await fetch('/Cart/GetCartCount');
            const cartData = await cartResponse.json();
            document.getElementById("cart-counter").textContent = cartData.count;

            const wishlistResponse = await fetch('/Wishlist/GetWishlistCount');
            const wishlistData = await wishlistResponse.json();
            document.getElementById("wishlist-counter").textContent = wishlistData.count;
        } catch (error) {
            console.error("Error updating counters:", error);
        }
    }
    updateCounters();
    setInterval(updateCounters, 10000);
});
