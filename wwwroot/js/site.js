// Common functions for the Car Rental application
$(document).ready(function () {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize date inputs with min date as today
    $('input[type="date"]').each(function () {
        $(this).attr('min', new Date().toISOString().split('T')[0]);
    });

    // Booking date validation
    $('#StartDate, #EndDate').on('change', function () {
        var startDate = new Date($('#StartDate').val());
        var endDate = new Date($('#EndDate').val());

        if (startDate && endDate) {
            if (startDate > endDate) {
                alert('End date must be after start date');
                $('#EndDate').val('');
                return;
            }

            // Update minimum end date based on start date
            $('#EndDate').attr('min', $('#StartDate').val());

            // Calculate total price if daily rate is available
            var dailyRate = parseFloat($('#DailyRate').val());
            if (!isNaN(dailyRate)) {
                var days = Math.ceil((endDate - startDate) / (1000 * 60 * 60 * 24));
                var total = days * dailyRate;
                $('#TotalPrice').text('$' + total.toFixed(2));
                $('#numberOfDays').text(days);
            }
        }
    });

    // Image error handling
    $('img').on('error', function () {
        $(this).attr('src', '/images/default-car.jpg');
    });

    // Search functionality
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('.car-card').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    // Sort functionality
    $('#sortSelect').on('change', function () {
        var sortBy = $(this).val();
        var carCards = $('.car-card').get();

        carCards.sort(function (a, b) {
            var aValue, bValue;

            switch (sortBy) {
                case 'priceAsc':
                    aValue = parseFloat($(a).data('price'));
                    bValue = parseFloat($(b).data('price'));
                    return aValue - bValue;
                case 'priceDesc':
                    aValue = parseFloat($(a).data('price'));
                    bValue = parseFloat($(b).data('price'));
                    return bValue - aValue;
                case 'nameAsc':
                    aValue = $(a).data('name');
                    bValue = $(b).data('name');
                    return aValue.localeCompare(bValue);
                case 'nameDesc':
                    aValue = $(a).data('name');
                    bValue = $(b).data('name');
                    return bValue.localeCompare(aValue);
            }
        });

        var carList = $('.car-list');
        $.each(carCards, function (index, item) {
            carList.append(item);
        });
    });

    // Confirmation dialogs
    $('.delete-confirm').on('click', function (e) {
        if (!confirm('Are you sure you want to delete this item?')) {
            e.preventDefault();
        }
    });

    $('.cancel-confirm').on('click', function (e) {
        if (!confirm('Are you sure you want to cancel this booking?')) {
            e.preventDefault();
        }
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').alert('close');
    }, 5000);

    // Add loading indicator
    $(document).on('submit', 'form', function () {
        var submitButton = $(this).find('button[type="submit"]');
        submitButton.prop('disabled', true);
        submitButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
    });

    // Enable form validation styles
    $('form').each(function () {
        $(this).on('submit', function () {
            $(this).addClass('was-validated');
        });
    });
});
