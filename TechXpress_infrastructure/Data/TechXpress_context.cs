using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;

namespace TechXpress_infrastructure.Data
{
    public class TechXpress_context : IdentityDbContext<ApplicationUser>
    {
        public TechXpress_context(DbContextOptions<TechXpress_context> options)
            : base(options)
        {
        }

        // DbSet declarations for all entities
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<LoginHistoryEntry> LoginHistoryEntries { get; set; }
        public DbSet<ActiveSession> ActiveSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes for query performance
            modelBuilder.Entity<Product>().HasIndex(p => p.Name);
            modelBuilder.Entity<Category>().HasIndex(c => c.Name);
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderDate);

            // 1. ApplicationUser & UserProfile (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.UserProfile)
                .WithOne(up => up.ApplicationUser)
                .HasForeignKey<UserProfile>(up => up.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. UserProfile & Addresses (One-to-Many)
            modelBuilder.Entity<UserProfile>()
                .HasMany(up => up.Addresses)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Category & Products (One-to-Many)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. ApplicationUser & Cart (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. ApplicationUser & Wishlist (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Wishlist)
                .WithOne(w => w.ApplicationUser)
                .HasForeignKey<Wishlist>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 6. WishlistItem (Composite Key & Relationships)
            modelBuilder.Entity<WishlistItem>()
                .HasKey(wi => new { wi.WishlistId, wi.ProductId });

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Wishlist)
                .WithMany(w => w.WishlistItems)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 7. CartItem (Composite Key & Relationships)
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 8. Orders & ApplicationUser (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ApplicationUser)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 9. Orders & OrderItems (One-to-Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 10. OrderItem & Product (One-to-Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 11. Shipping & Order (One-to-One)
            modelBuilder.Entity<Shipping>()
                .HasOne(s => s.Order)
                .WithOne(o => o.Shipping)
                .HasForeignKey<Shipping>(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 12. Reviews & Product (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 13. Reviews & ApplicationUser (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 14. ProductImages: Product has many ProductImages.
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductImage>().HasData(
    // Product 1
    new ProductImage { Id = 1, ProductId = 1, ImageUrl = "https://th.bing.com/th/id/R.f9b7e4f64d30ecd7020860db1f89bb94?rik=CzEbAPGXFoZUZg&pid=ImgRaw&r=0" },
    new ProductImage { Id = 2, ProductId = 1, ImageUrl = "https://www.genesink.com/wp-content/uploads/2019/03/display-vignette.jpg" },
    new ProductImage { Id = 3, ProductId = 1, ImageUrl = "https://th.bing.com/th/id/R.714e4d18d0c3392a0b41d063f0ae9228?rik=br4ovv8sBZPdBQ&riu=http%3a%2f%2fwww.giaitech.com.cn%2fwp-content%2fuploads%2f20160217001710322.jpg&ehk=7lw9IURGFO1GTdD1fAioHSOXwVykGuxLIDXGYIAuN9s%3d&risl=&pid=ImgRaw&r=0" },

    // Product 2
    new ProductImage { Id = 4, ProductId = 2, ImageUrl = "https://th.bing.com/th/id/OIP.RbXiEBlo0TjEwllefkKUngHaEK?rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 5, ProductId = 2, ImageUrl = "https://sm.mashable.com/t/mashable_in/review/l/lenovo-yog/lenovo-yoga-c940-14-inch-review-a-powerful-laptop-that-adapt_698k.960.jpg" },
    new ProductImage { Id = 6, ProductId = 2, ImageUrl = "https://www.stuff.tv/wp-content/uploads/sites/2/2022/10/Lenovo-Yoga-Slim-9i-lead.jpg?w=1080" },

    // Product 3
    new ProductImage { Id = 7, ProductId = 3, ImageUrl = "https://cdn.mos.cms.futurecdn.net/bP2RiEaSMDEkCV6jN7o4JM-1280-80.jpg" },
    new ProductImage { Id = 8, ProductId = 3, ImageUrl = "https://th.bing.com/th/id/OIP.7RoVQDUJu8-JmfPTyAuu1QHaE8?pid=ImgDet&w=474&h=316&rs=1" },
    new ProductImage { Id = 9, ProductId = 3, ImageUrl = "https://th.bing.com/th/id/OIP.Ny7wVnZcwlxAtVJio9W8agHaE8?pid=ImgDet&w=474&h=316&rs=1" },

    // Product 4
    new ProductImage { Id = 10, ProductId = 4, ImageUrl = "https://th.bing.com/th/id/OIP.Bin9XqWpiw52DFAu4vK6ngHaHa?rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 11, ProductId = 4, ImageUrl = "https://images5.tanganetwork.com/prod?bucket=tanga-fetched-images-prod&filename=5848e000-8105-11e7-b99a-ab8f21c63662.jpg&width=635&height=635&quality=90" },
    new ProductImage { Id = 12, ProductId = 4, ImageUrl = "https://th.bing.com/th/id/OIP.xNpbYvCqv6zVkycime3ZrQHaGy?w=655&h=600&rs=1&pid=ImgDetMain" },

    // Product 5
    new ProductImage { Id = 13, ProductId = 5, ImageUrl = "https://a-static.mlcdn.com.br/1500x1500/smart-tv-55-crystal-4k-samsung-55au8000-wi-fi-bluetooth-hdr-alexa-built-in-3-hdmi-2-usb/magazineluiza/193441900/7fabc39533e941e2c669887a003e6a4f.jpg" },
    new ProductImage { Id = 14, ProductId = 5, ImageUrl = "https://th.bing.com/th/id/OIP.WgYZbwCh_7ILVpJTeAjaQQHaHP?w=2943&h=2879&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 15, ProductId = 5, ImageUrl = "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/6401/6401738cv1d.jpg" },

    // Product 7
    new ProductImage { Id = 16, ProductId = 7, ImageUrl = "https://th.bing.com/th/id/OIP.05qN8o496JUUFxEcno1DMgAAAA?rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 17, ProductId = 7, ImageUrl = "https://th.bing.com/th/id/OIP.dfsGUIL7GLKPvs2_NfqRCQHaHa?w=710&h=710&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 18, ProductId = 7, ImageUrl = "https://c1.neweggimages.com/ProductImage/96-811-092-01.jpg" },

    // Product 8
    new ProductImage { Id = 19, ProductId = 8, ImageUrl = "https://i5.walmartimages.com/asr/903ecb9c-b1f4-47ad-a8e6-02813adda405.a074dc6a0ebbb7580d2a9e7594e030ab.jpeg?odnWidth=612&odnHeight=612&odnBg=ffffff" },
    new ProductImage { Id = 20, ProductId = 8, ImageUrl = "https://ae01.alicdn.com/kf/HTB1HyPdgfNZWeJjSZFpq6xjBFXaw/ZEALOT-B19-HiFi-Bluetooth-Headphones-Foldable-Wireless-Stereo-Earphone-Headsets-with-Mic-Micro-SD-Card-Slot.jpg_640x640.jpg" },
    new ProductImage { Id = 21, ProductId = 8, ImageUrl = "https://ae01.alicdn.com/kf/HTB1RejuXmYTBKNjSZKbq6xJ8pXaE/B19-Bluetooth-Headphones-Wireless-Stereo-HiFi-Music-Headphone-with-Mic-Headsets-Micro-SD-Card-Slot-FM.jpg" },

    // Product 9
    new ProductImage { Id = 22, ProductId = 9, ImageUrl = "https://cdn.vox-cdn.com/uploads/chorus_image/image/72828338/wh5g758d.0.png" },
    new ProductImage { Id = 23, ProductId = 9, ImageUrl = "https://gagadget.com/media/cache/83/7d/837dd2b7cfaab35157c0c4bfb7989e61.jpg" },
    new ProductImage { Id = 24, ProductId = 9, ImageUrl = "https://cdn.mos.cms.futurecdn.net/PynLBY7LkNLNQ4uu44xwjc.jpg" },

    // Product 10
    new ProductImage { Id = 25, ProductId = 10, ImageUrl = "https://m.media-amazon.com/images/I/817NiRrxXUL.jpg" },
    new ProductImage { Id = 26, ProductId = 10, ImageUrl = "https://m.media-amazon.com/images/I/41Xf2i6uRGL._SL500_.jpg" },
    new ProductImage { Id = 27, ProductId = 10, ImageUrl = "https://media.diy.com/is/image/KingfisherDigital/kitchen-perfected-eco-friendly-blue-illuminating-cordless-glass-kettle-1-7ltr-2200w~5052337012572_01c_MP?$MOB_PREV$&$width=768&$height=768" },

    // Product 11
    new ProductImage { Id = 28, ProductId = 11, ImageUrl = "https://th.bing.com/th/id/OIP.aQ_uIV3Q_-CE7YvDOaj1RgHaHa?rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 29, ProductId = 11, ImageUrl = "https://ae01.alicdn.com/kf/HTB13dALPFXXXXcxXFXXq6xXFXXXa/Smart-Bracelet-Bracelets-Fitness-Tracker-Band-Wristband-Wearable-Devices-Watch-Pulsera-Inteligente-Pulse-Monitor-Pk-Xiomi.jpg" },
    new ProductImage { Id = 30, ProductId = 11, ImageUrl = "https://th.bing.com/th/id/OIP.k8ijMJxs-Kmr4-3hekQGlgHaHa?w=500&h=500&rs=1&pid=ImgDetMain" },

    // Product 12
    new ProductImage { Id = 31, ProductId = 12, ImageUrl = "https://th.bing.com/th/id/OIP.PsDbUPIRgi5r3En0Wu_TLQAAAA?w=474&h=474&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 32, ProductId = 12, ImageUrl = "https://i0.wp.com/bestbargains.lk/wp-content/uploads/2021/01/maxmo-air-fryer-sri-lanka.jpg?w=720&ssl=1" },
    new ProductImage { Id = 33, ProductId = 12, ImageUrl = "https://totalrvandcamping.com.au/wp-content/uploads/2023/09/301485_1_0gai9yk11j6ymwf9-1024x1024.jpg" },

    // Product 13
    new ProductImage { Id = 34, ProductId = 13, ImageUrl = "https://www.goodontop.com/images/foods/low-card-foods/speakers/tribit-ts-bts20-xsound-go-bluetooth-speaker-by-tribit.jpg" },
    new ProductImage { Id = 35, ProductId = 13, ImageUrl = "https://th.bing.com/th/id/OIP.F5x-7Dkp5YuOsJAw1V0NjAHaGS?w=1000&h=849&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 36, ProductId = 13, ImageUrl = "https://robots.net/wp-content/uploads/2019/07/2_9_Tribit-XSound-Go-portable-Speakers.jpeg" },

    // Product 14
    new ProductImage { Id = 37, ProductId = 14, ImageUrl = "https://th.bing.com/th/id/R.754d75dc6939872b63d7ae2be6dd27ac?rik=R%2bXH60jc2XXrCg&pid=ImgRaw&r=0" },
    new ProductImage { Id = 38, ProductId = 14, ImageUrl = "https://th.bing.com/th/id/OIP.aT5wnR6ncddbkQ00gY0AIAHaFn?w=1350&h=1023&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 39, ProductId = 14, ImageUrl = "https://th.bing.com/th/id/OIP.0QjMD0CM8JWA-xUEmBtYiwAAAA?w=474&h=474&rs=1&pid=ImgDetMain" },

    // Product 15
    new ProductImage { Id = 40, ProductId = 15, ImageUrl = "https://th.bing.com/th/id/R.8b855b77b3e79f9d969b4d3b563ec0e5?rik=F%2faC7BCXMYe9qw&pid=ImgRaw&r=0" },
    new ProductImage { Id = 41, ProductId = 15, ImageUrl = "https://ae01.alicdn.com/kf/Hb804706445c749b5b98231b728f1fc79l/JONSBO-TW4-360-RGB-CPU-water-cooling-14-water-channels-Shenguang-synchronization-ARGB-controller-with-remote.jpg" },
    new ProductImage { Id = 42, ProductId = 15, ImageUrl = "https://th.bing.com/th/id/OIP.CzBmj8oFONV0Ol7LS7sRqgHaDT?rs=1&pid=ImgDetMain" },

    // Product 16
    new ProductImage { Id = 43, ProductId = 16, ImageUrl = "https://whiteaways.lk/wp-content/uploads/2023/08/HIMA.jpg" },
    new ProductImage { Id = 44, ProductId = 16, ImageUrl = "https://technicalustad.com/wp-content/uploads/2022/09/best-micro-atx-case-new-3-295x300.jpg" },
    new ProductImage { Id = 45, ProductId = 16, ImageUrl = "https://s.alicdn.com/@sc04/kf/H16440c708e4d4dbf93018df44dfb5e3eS.png_300x300.png" },

    // Product 17
    new ProductImage { Id = 46, ProductId = 17, ImageUrl = "https://th.bing.com/th/id/OIP.1viVJI6lll5-tRlXvlCA-QHaHa?w=800&h=800&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 47, ProductId = 17, ImageUrl = "https://www.makotekcomputers.com/cdn/shop/files/ASUS-ROG-STRIX-Z590-E-GAMING-WIFI-12-MONTHS-WARRANTY-MOTHERBOARD.png?v=1715388173" },
    new ProductImage { Id = 48, ProductId = 17, ImageUrl = "https://th.bing.com/th/id/OIP.QMlbqRJ4WNyUxVjGZs5B1AHaHa?w=1000&h=1000&rs=1&pid=ImgDetMain" },

    // Product 18
    new ProductImage { Id = 49, ProductId = 18, ImageUrl = "https://th.bing.com/th/id/R.47ed04d9edb9483bb4578b82a7067ecf?rik=IvcOiG5amnNtmA&pid=ImgRaw&r=0" },
    new ProductImage { Id = 50, ProductId = 18, ImageUrl = "https://gepig.com/news/4038.jpg" },
    new ProductImage { Id = 51, ProductId = 18, ImageUrl = "https://th.bing.com/th/id/OIP.J524we_sLclBNYEOIGOppAAAAA?rs=1&pid=ImgDetMain" },

    // Product 19
    new ProductImage { Id = 52, ProductId = 19, ImageUrl = "https://th.bing.com/th/id/R.dd66a48254aca2d1e37b8887993a100c?rik=Ileg6QdLFs2iTA&pid=ImgRaw&r=0" },
    new ProductImage { Id = 53, ProductId = 19, ImageUrl = "https://th.bing.com/th/id/OIP.zz1pXOlbd6blFLpXQoJdxgHaEY?w=900&h=533&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 54, ProductId = 19, ImageUrl = "https://th.bing.com/th/id/OIP.YL2XY4-NUxdg4bo-AoTHfgHaD4?rs=1&pid=ImgDetMain" },

    // Product 20
    new ProductImage { Id = 55, ProductId = 20, ImageUrl = "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/6443/6443448_sd.jpg" },
    new ProductImage { Id = 56, ProductId = 20, ImageUrl = "https://www.gsm-helmond.nl/wp-content/uploads/2021/09/ip13p.png" },
    new ProductImage { Id = 57, ProductId = 20, ImageUrl = "https://th.bing.com/th/id/OIP.0BSb4aDNgwz98tWiSzghmAHaHa?pid=ImgDet&w=474&h=474&rs=1" },

    // Product 21
    new ProductImage { Id = 58, ProductId = 21, ImageUrl = "https://th.bing.com/th/id/R.a71e552fa9db627e970d4f9265b2bdea?rik=7c2q579igV01%2fw&riu=http%3a%2f%2f1.bp.blogspot.com%2f-8Sqoo-pIH44%2fVej65FF82dI%2fAAAAAAAAAMs%2fK1B1Jnijekk%2fs1600%2fbridge.jpg&ehk=Ri%2bfgoOprMK5WCr5iSwegmqi33uUKgYBsmYAwydbJIc%3d&risl=&pid=ImgRaw&r=0" },
    new ProductImage { Id = 59, ProductId = 21, ImageUrl = "https://www.bdstall.com/asset/product-image/giant_15949.jpg" },
    new ProductImage { Id = 60, ProductId = 21, ImageUrl = "https://i1.wp.com/www.itechnews.net/wp-content/uploads/2013/03/FujiFilm-FinePix-S8400W-44x-Long-Zoom-Camera-supports-WiFi-zooming-angle.jpg?fit=600%2C552" },

    // Product 22
    new ProductImage { Id = 61, ProductId = 22, ImageUrl = "https://th.bing.com/th/id/OIP.hO3M_lb6JnlfCzEfQad01wHaH0?rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 62, ProductId = 22, ImageUrl = "https://th.bing.com/th/id/OIP.ymSNRvHme27UC7q_gRxM9QHaJi?w=720&h=928&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 63, ProductId = 22, ImageUrl = "https://i.ebayimg.com/images/g/UiYAAOSwz29kga0n/s-l1600.jpg" },

    // Product 23
    new ProductImage { Id = 64, ProductId = 23, ImageUrl = "https://th.bing.com/th/id/OIP.dUwz42xWONVV0_iixdxItAHaHa?w=646&h=646&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 65, ProductId = 23, ImageUrl = "https://th.bing.com/th/id/OIP.JUprBEeYu8BSQviFHdwLXwHaIZ?w=679&h=770&rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 66, ProductId = 23, ImageUrl = "https://www.buyhomeappliance.co.uk/wp-content/uploads/2021/09/RF750N4ISF_3-scaled-1.jpg" },

    // Product 24
    new ProductImage { Id = 67, ProductId = 24, ImageUrl = "https://cdn.taw9eel.com/media/catalog/product/cache/1/image/519x/9df78eab33525d08d6e5fb8d27136e95/f/t/ftgc4565-en.jpg" },
    new ProductImage { Id = 68, ProductId = 24, ImageUrl = "https://scene7.samsclub.com/is/image/samsclub/0071171957337_A" },
    new ProductImage { Id = 69, ProductId = 24, ImageUrl = "https://cdn.media.amplience.net/i/xcite/545474-03?img404=default&w=2048&qlt=75&fmt=auto" },

    // Product 25
    new ProductImage { Id = 70, ProductId = 25, ImageUrl = "https://th.bing.com/th/id/R.831423052c67d6782e5c569149c22030?rik=TcoxTyim18pSxQ&pid=ImgRaw&r=0" },
    new ProductImage { Id = 71, ProductId = 25, ImageUrl = "https://gh.jumia.is/unsafe/fit-in/680x680/filters:fill(white)/product/60/0736041/1.jpg?2666" },
    new ProductImage { Id = 72, ProductId = 25, ImageUrl = "https://m.media-amazon.com/images/I/51JdeBBTHKS._AC_SL1500_.jpg" },

    // Product 26
    new ProductImage { Id = 73, ProductId = 26, ImageUrl = "https://th.bing.com/th/id/OIP.hXrpTB9ISOzzr37bnsCR1AHaFC?rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 74, ProductId = 26, ImageUrl = "https://th.bing.com/th/id/OIP.nB_hHWbpe_jru29GCJ9mJQHaFK?rs=1&pid=ImgDetMain" },
    new ProductImage { Id = 75, ProductId = 26, ImageUrl = "https://www.pricerunner.dk/product/1200x630/3000665543/HP-Officejet-Pro-9013.jpg" }
);



        }
    }
}
