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

    async function updateCartAndWishlistCounts() {
        try {
            // Fetch cart count (ensure your CartController has a similar endpoint)
            const cartResponse = await fetch('/Cart/GetCartCount');
            if (cartResponse.ok) {
                const cartData = await cartResponse.json();
                const cartCounter = document.getElementById("cart-counter");
                if (cartCounter) {
                    cartCounter.innerText = cartData.count;
                }
            } else {
                console.error("Failed to fetch cart count");
            }

            // Fetch wishlist count
            const wishlistResponse = await fetch('/Wishlist/GetWishlistCount');
            if (wishlistResponse.ok) {
                const wishlistData = await wishlistResponse.json();
                const wishlistCounter = document.getElementById("wishlist-counter");
                if (wishlistCounter) {
                    wishlistCounter.innerText = wishlistData.count;
                }
            } else {
                console.error("Failed to fetch wishlist count");
            }
        } catch (error) {
            console.error("Error updating counts:", error);
        }
    }

    updateCartAndWishlistCounts();
    setInterval(updateCartAndWishlistCounts, 10000); // Update every 10 second

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
                if (target) {
                    target.scrollIntoView({ behavior: "smooth", block: "start" });
                }
            }
        });
    });

    // ----- Modal Functionality -----
    window.openModal = (name, description, price, imageUrl, productId) => {
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

    window.closeModal = () => {
        const modal = document.getElementById("productModal");
        modal.classList.remove("active");
        setTimeout(() => { modal.style.display = "none"; }, 300);
    };

    window.updateQuantity = (change) => {
        const quantityInput = document.getElementById("quantity");
        let currentValue = parseInt(quantityInput.value);
        let newValue = currentValue + change;
        newValue = Math.max(1, Math.min(newValue, 99)); // Limit quantity between 1 and 99
        quantityInput.value = newValue;
    };

    const modalElement = document.getElementById("productModal");
    if (modalElement) {
        modalElement.addEventListener("click", (e) => {
            if (e.target === modalElement) {
                window.closeModal();
            }
        });
        document.addEventListener("keydown", (e) => {
            if (e.key === "Escape" && modalElement.style.display === "flex") {
                window.closeModal();
            }
        });
    }

    // ----- Category Dropdown Toggle -----
    const catTrigger = document.getElementById("categoryTrigger");
    const catDropdown = document.getElementById("categoryDropdown");

    if (catTrigger && catDropdown) {
        catTrigger.addEventListener("click", function (e) {
            e.preventDefault();
            const isExpanded = catDropdown.classList.toggle("show");
            catTrigger.setAttribute("aria-expanded", isExpanded);
        });

        // Close dropdown when clicking outside
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
        stopScrolling(); // Clear any existing intervals
        scrollInterval = setInterval(() => {
            catDropdown.scrollBy({ left: direction * 50, behavior: 'auto' });
        }, 20);
    };

    const stopScrolling = () => {
        clearInterval(scrollInterval);
    };

    catDropdown.addEventListener('mouseover', (e) => {
        const rect = catDropdown.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;

        if (mouseX < 50) {
            startScrolling(-1); // Scroll left
        } else if (mouseX > rect.width - 50) {
            startScrolling(1); // Scroll right
        }
    });

    catDropdown.addEventListener('mousemove', (e) => {
        const rect = catDropdown.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;

        if (mouseX < 50) {
            startScrolling(-1); // Scroll left
        } else if (mouseX > rect.width - 50) {
            startScrolling(1); // Scroll right
        } else {
            stopScrolling();
        }
    });

    catDropdown.addEventListener('mouseleave', stopScrolling);
});