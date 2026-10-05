// =====================================================
// COMMON UI
// =====================================================

$(function () {

    // Current year
    var yearElement = document.querySelector("#displayYear");
    if (yearElement) {
        yearElement.innerHTML = new Date().getFullYear();
    }


    // =================================================
    // ISOTOPE MENU FILTER
    // =================================================

    if ($.fn.isotope && $(".grid").length) {

        var $grid = $(".grid").isotope({
            itemSelector: ".all",
            percentPosition: false,
            masonry: {
                columnWidth: ".all"
            }
        });

        $(document).on("click", ".filters_menu li", function () {
            $(".filters_menu li").removeClass("active");
            $(this).addClass("active");

            var filterValue = $(this).attr("data-filter");

            $grid.isotope({
                filter: filterValue
            });
        });
    }


    // =================================================
    // CUSTOMER REVIEW CAROUSEL
    // =================================================

    if ($.fn.owlCarousel && $(".client_owl-carousel").length) {

        $(".client_owl-carousel").each(function () {

            var $carousel = $(this);

            if ($carousel.hasClass("owl-loaded")) {
                return;
            }

            $carousel.owlCarousel({

                loop: true,

                margin: 20,

                nav: true,

                dots: false,

                autoplay: true,

                autoplayTimeout: 4000,

                autoplayHoverPause: true,

                smartSpeed: 700,

                navText: [
                    '<i class="fa fa-angle-left" aria-hidden="true"></i>',
                    '<i class="fa fa-angle-right" aria-hidden="true"></i>'
                ],

                responsive: {
                    0: {
                        items: 1
                    },
                    768: {
                        items: 2
                    },
                    1200: {
                        items: 2
                    }
                }

            });

        });
    }


    // =================================================
    // REVIEW STAR SELECTOR
    // Works for dynamically loaded Menu Details too.
    // =================================================

    $(document).on("click", ".review-rating-item", function () {

        var rating = parseInt($(this).attr("data-rating"), 10);

        if (!rating) {
            return;
        }

        var $selector = $(this).closest(".review-rating-selector");

        $selector.find("input[type='radio']").prop("checked", false);
        $(this).find("input[type='radio']").prop("checked", true);

        $selector.find(".review-rating-item i").each(function () {

            var value = parseInt($(this).closest(".review-rating-item").attr("data-rating"), 10);

            $(this)
                .removeClass("fa-star fa-star-o text-warning text-muted")
                .addClass(value <= rating
                    ? "fa-star text-warning"
                    : "fa-star-o text-muted");

        });
    });

});


// =====================================================
// GOOGLE MAP
// =====================================================

function myMap() {

    var mapElement = document.getElementById("googleMap");

    if (!mapElement || typeof google === "undefined") {
        return;
    }

    var mapProp = {
        center: new google.maps.LatLng(10.7769, 106.7009),
        zoom: 15
    };

    new google.maps.Map(mapElement, mapProp);
}
