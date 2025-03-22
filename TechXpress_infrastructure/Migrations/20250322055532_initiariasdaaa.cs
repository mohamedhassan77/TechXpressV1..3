using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechXpress_infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initiariasdaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "ImageUrl", "ProductId" },
                values: new object[,]
                {
                    { 1, "https://th.bing.com/th/id/R.f9b7e4f64d30ecd7020860db1f89bb94?rik=CzEbAPGXFoZUZg&pid=ImgRaw&r=0", 1 },
                    { 2, "https://www.genesink.com/wp-content/uploads/2019/03/display-vignette.jpg", 1 },
                    { 3, "https://th.bing.com/th/id/R.714e4d18d0c3392a0b41d063f0ae9228?rik=br4ovv8sBZPdBQ&riu=http%3a%2f%2fwww.giaitech.com.cn%2fwp-content%2fuploads%2f20160217001710322.jpg&ehk=7lw9IURGFO1GTdD1fAioHSOXwVykGuxLIDXGYIAuN9s%3d&risl=&pid=ImgRaw&r=0", 1 },
                    { 4, "https://th.bing.com/th/id/OIP.RbXiEBlo0TjEwllefkKUngHaEK?rs=1&pid=ImgDetMain", 2 },
                    { 5, "https://sm.mashable.com/t/mashable_in/review/l/lenovo-yog/lenovo-yoga-c940-14-inch-review-a-powerful-laptop-that-adapt_698k.960.jpg", 2 },
                    { 6, "https://www.stuff.tv/wp-content/uploads/sites/2/2022/10/Lenovo-Yoga-Slim-9i-lead.jpg?w=1080", 2 },
                    { 7, "https://cdn.mos.cms.futurecdn.net/bP2RiEaSMDEkCV6jN7o4JM-1280-80.jpg", 3 },
                    { 8, "https://th.bing.com/th/id/OIP.7RoVQDUJu8-JmfPTyAuu1QHaE8?pid=ImgDet&w=474&h=316&rs=1", 3 },
                    { 9, "https://th.bing.com/th/id/OIP.Ny7wVnZcwlxAtVJio9W8agHaE8?pid=ImgDet&w=474&h=316&rs=1", 3 },
                    { 10, "https://th.bing.com/th/id/OIP.Bin9XqWpiw52DFAu4vK6ngHaHa?rs=1&pid=ImgDetMain", 4 },
                    { 11, "https://images5.tanganetwork.com/prod?bucket=tanga-fetched-images-prod&filename=5848e000-8105-11e7-b99a-ab8f21c63662.jpg&width=635&height=635&quality=90", 4 },
                    { 12, "https://th.bing.com/th/id/OIP.xNpbYvCqv6zVkycime3ZrQHaGy?w=655&h=600&rs=1&pid=ImgDetMain", 4 },
                    { 13, "https://a-static.mlcdn.com.br/1500x1500/smart-tv-55-crystal-4k-samsung-55au8000-wi-fi-bluetooth-hdr-alexa-built-in-3-hdmi-2-usb/magazineluiza/193441900/7fabc39533e941e2c669887a003e6a4f.jpg", 5 },
                    { 14, "https://th.bing.com/th/id/OIP.WgYZbwCh_7ILVpJTeAjaQQHaHP?w=2943&h=2879&rs=1&pid=ImgDetMain", 5 },
                    { 15, "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/6401/6401738cv1d.jpg", 5 },
                    { 16, "https://th.bing.com/th/id/OIP.05qN8o496JUUFxEcno1DMgAAAA?rs=1&pid=ImgDetMain", 7 },
                    { 17, "https://th.bing.com/th/id/OIP.dfsGUIL7GLKPvs2_NfqRCQHaHa?w=710&h=710&rs=1&pid=ImgDetMain", 7 },
                    { 18, "https://c1.neweggimages.com/ProductImage/96-811-092-01.jpg", 7 },
                    { 19, "https://i5.walmartimages.com/asr/903ecb9c-b1f4-47ad-a8e6-02813adda405.a074dc6a0ebbb7580d2a9e7594e030ab.jpeg?odnWidth=612&odnHeight=612&odnBg=ffffff", 8 },
                    { 20, "https://ae01.alicdn.com/kf/HTB1HyPdgfNZWeJjSZFpq6xjBFXaw/ZEALOT-B19-HiFi-Bluetooth-Headphones-Foldable-Wireless-Stereo-Earphone-Headsets-with-Mic-Micro-SD-Card-Slot.jpg_640x640.jpg", 8 },
                    { 21, "https://ae01.alicdn.com/kf/HTB1RejuXmYTBKNjSZKbq6xJ8pXaE/B19-Bluetooth-Headphones-Wireless-Stereo-HiFi-Music-Headphone-with-Mic-Headsets-Micro-SD-Card-Slot-FM.jpg", 8 },
                    { 22, "https://cdn.vox-cdn.com/uploads/chorus_image/image/72828338/wh5g758d.0.png", 9 },
                    { 23, "https://gagadget.com/media/cache/83/7d/837dd2b7cfaab35157c0c4bfb7989e61.jpg", 9 },
                    { 24, "https://cdn.mos.cms.futurecdn.net/PynLBY7LkNLNQ4uu44xwjc.jpg", 9 },
                    { 25, "https://m.media-amazon.com/images/I/817NiRrxXUL.jpg", 10 },
                    { 26, "https://m.media-amazon.com/images/I/41Xf2i6uRGL._SL500_.jpg", 10 },
                    { 27, "https://media.diy.com/is/image/KingfisherDigital/kitchen-perfected-eco-friendly-blue-illuminating-cordless-glass-kettle-1-7ltr-2200w~5052337012572_01c_MP?$MOB_PREV$&$width=768&$height=768", 10 },
                    { 28, "https://th.bing.com/th/id/OIP.aQ_uIV3Q_-CE7YvDOaj1RgHaHa?rs=1&pid=ImgDetMain", 11 },
                    { 29, "https://ae01.alicdn.com/kf/HTB13dALPFXXXXcxXFXXq6xXFXXXa/Smart-Bracelet-Bracelets-Fitness-Tracker-Band-Wristband-Wearable-Devices-Watch-Pulsera-Inteligente-Pulse-Monitor-Pk-Xiomi.jpg", 11 },
                    { 30, "https://th.bing.com/th/id/OIP.k8ijMJxs-Kmr4-3hekQGlgHaHa?w=500&h=500&rs=1&pid=ImgDetMain", 11 },
                    { 31, "https://th.bing.com/th/id/OIP.PsDbUPIRgi5r3En0Wu_TLQAAAA?w=474&h=474&rs=1&pid=ImgDetMain", 12 },
                    { 32, "https://i0.wp.com/bestbargains.lk/wp-content/uploads/2021/01/maxmo-air-fryer-sri-lanka.jpg?w=720&ssl=1", 12 },
                    { 33, "https://totalrvandcamping.com.au/wp-content/uploads/2023/09/301485_1_0gai9yk11j6ymwf9-1024x1024.jpg", 12 },
                    { 34, "https://www.goodontop.com/images/foods/low-card-foods/speakers/tribit-ts-bts20-xsound-go-bluetooth-speaker-by-tribit.jpg", 13 },
                    { 35, "https://th.bing.com/th/id/OIP.F5x-7Dkp5YuOsJAw1V0NjAHaGS?w=1000&h=849&rs=1&pid=ImgDetMain", 13 },
                    { 36, "https://robots.net/wp-content/uploads/2019/07/2_9_Tribit-XSound-Go-portable-Speakers.jpeg", 13 },
                    { 37, "https://th.bing.com/th/id/R.754d75dc6939872b63d7ae2be6dd27ac?rik=R%2bXH60jc2XXrCg&pid=ImgRaw&r=0", 14 },
                    { 38, "https://th.bing.com/th/id/OIP.aT5wnR6ncddbkQ00gY0AIAHaFn?w=1350&h=1023&rs=1&pid=ImgDetMain", 14 },
                    { 39, "https://th.bing.com/th/id/OIP.0QjMD0CM8JWA-xUEmBtYiwAAAA?w=474&h=474&rs=1&pid=ImgDetMain", 14 },
                    { 40, "https://th.bing.com/th/id/R.8b855b77b3e79f9d969b4d3b563ec0e5?rik=F%2faC7BCXMYe9qw&pid=ImgRaw&r=0", 15 },
                    { 41, "https://ae01.alicdn.com/kf/Hb804706445c749b5b98231b728f1fc79l/JONSBO-TW4-360-RGB-CPU-water-cooling-14-water-channels-Shenguang-synchronization-ARGB-controller-with-remote.jpg", 15 },
                    { 42, "https://th.bing.com/th/id/OIP.CzBmj8oFONV0Ol7LS7sRqgHaDT?rs=1&pid=ImgDetMain", 15 },
                    { 43, "https://whiteaways.lk/wp-content/uploads/2023/08/HIMA.jpg", 16 },
                    { 44, "https://technicalustad.com/wp-content/uploads/2022/09/best-micro-atx-case-new-3-295x300.jpg", 16 },
                    { 45, "https://s.alicdn.com/@sc04/kf/H16440c708e4d4dbf93018df44dfb5e3eS.png_300x300.png", 16 },
                    { 46, "https://th.bing.com/th/id/OIP.1viVJI6lll5-tRlXvlCA-QHaHa?w=800&h=800&rs=1&pid=ImgDetMain", 17 },
                    { 47, "https://www.makotekcomputers.com/cdn/shop/files/ASUS-ROG-STRIX-Z590-E-GAMING-WIFI-12-MONTHS-WARRANTY-MOTHERBOARD.png?v=1715388173", 17 },
                    { 48, "https://th.bing.com/th/id/OIP.QMlbqRJ4WNyUxVjGZs5B1AHaHa?w=1000&h=1000&rs=1&pid=ImgDetMain", 17 },
                    { 49, "https://th.bing.com/th/id/R.47ed04d9edb9483bb4578b82a7067ecf?rik=IvcOiG5amnNtmA&pid=ImgRaw&r=0", 18 },
                    { 50, "https://gepig.com/news/4038.jpg", 18 },
                    { 51, "https://th.bing.com/th/id/OIP.J524we_sLclBNYEOIGOppAAAAA?rs=1&pid=ImgDetMain", 18 },
                    { 52, "https://th.bing.com/th/id/R.dd66a48254aca2d1e37b8887993a100c?rik=Ileg6QdLFs2iTA&pid=ImgRaw&r=0", 19 },
                    { 53, "https://th.bing.com/th/id/OIP.zz1pXOlbd6blFLpXQoJdxgHaEY?w=900&h=533&rs=1&pid=ImgDetMain", 19 },
                    { 54, "https://th.bing.com/th/id/OIP.YL2XY4-NUxdg4bo-AoTHfgHaD4?rs=1&pid=ImgDetMain", 19 },
                    { 55, "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/6443/6443448_sd.jpg", 20 },
                    { 56, "https://www.gsm-helmond.nl/wp-content/uploads/2021/09/ip13p.png", 20 },
                    { 57, "https://th.bing.com/th/id/OIP.0BSb4aDNgwz98tWiSzghmAHaHa?pid=ImgDet&w=474&h=474&rs=1", 20 },
                    { 58, "https://th.bing.com/th/id/R.a71e552fa9db627e970d4f9265b2bdea?rik=7c2q579igV01%2fw&riu=http%3a%2f%2f1.bp.blogspot.com%2f-8Sqoo-pIH44%2fVej65FF82dI%2fAAAAAAAAAMs%2fK1B1Jnijekk%2fs1600%2fbridge.jpg&ehk=Ri%2bfgoOprMK5WCr5iSwegmqi33uUKgYBsmYAwydbJIc%3d&risl=&pid=ImgRaw&r=0", 21 },
                    { 59, "https://www.bdstall.com/asset/product-image/giant_15949.jpg", 21 },
                    { 60, "https://i1.wp.com/www.itechnews.net/wp-content/uploads/2013/03/FujiFilm-FinePix-S8400W-44x-Long-Zoom-Camera-supports-WiFi-zooming-angle.jpg?fit=600%2C552", 21 },
                    { 61, "https://th.bing.com/th/id/OIP.hO3M_lb6JnlfCzEfQad01wHaH0?rs=1&pid=ImgDetMain", 22 },
                    { 62, "https://th.bing.com/th/id/OIP.ymSNRvHme27UC7q_gRxM9QHaJi?w=720&h=928&rs=1&pid=ImgDetMain", 22 },
                    { 63, "https://i.ebayimg.com/images/g/UiYAAOSwz29kga0n/s-l1600.jpg", 22 },
                    { 64, "https://th.bing.com/th/id/OIP.dUwz42xWONVV0_iixdxItAHaHa?w=646&h=646&rs=1&pid=ImgDetMain", 23 },
                    { 65, "https://th.bing.com/th/id/OIP.JUprBEeYu8BSQviFHdwLXwHaIZ?w=679&h=770&rs=1&pid=ImgDetMain", 23 },
                    { 66, "https://www.buyhomeappliance.co.uk/wp-content/uploads/2021/09/RF750N4ISF_3-scaled-1.jpg", 23 },
                    { 67, "https://cdn.taw9eel.com/media/catalog/product/cache/1/image/519x/9df78eab33525d08d6e5fb8d27136e95/f/t/ftgc4565-en.jpg", 24 },
                    { 68, "https://scene7.samsclub.com/is/image/samsclub/0071171957337_A", 24 },
                    { 69, "https://cdn.media.amplience.net/i/xcite/545474-03?img404=default&w=2048&qlt=75&fmt=auto", 24 },
                    { 70, "https://th.bing.com/th/id/R.831423052c67d6782e5c569149c22030?rik=TcoxTyim18pSxQ&pid=ImgRaw&r=0", 25 },
                    { 71, "https://gh.jumia.is/unsafe/fit-in/680x680/filters:fill(white)/product/60/0736041/1.jpg?2666", 25 },
                    { 72, "https://m.media-amazon.com/images/I/51JdeBBTHKS._AC_SL1500_.jpg", 25 },
                    { 73, "https://th.bing.com/th/id/OIP.hXrpTB9ISOzzr37bnsCR1AHaFC?rs=1&pid=ImgDetMain", 26 },
                    { 74, "https://th.bing.com/th/id/OIP.nB_hHWbpe_jru29GCJ9mJQHaFK?rs=1&pid=ImgDetMain", 26 },
                    { 75, "https://www.pricerunner.dk/product/1200x630/3000665543/HP-Officejet-Pro-9013.jpg", 26 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 75);
        }
    }
}
