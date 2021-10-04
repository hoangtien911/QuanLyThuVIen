// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart Example
var ctx = document.getElementById("myPieChart");
var Tong, DangMuon, DaTra, QuaHan;
var myPieChart;
$(document).ready(function () {
    $.ajax({
        url: "/Admin/ThongKe/DataThongKe",
        data: {
            status: "check",
        },
        dataType: "json",
        type: "POST",
        success: function (response) {
            Tong = response.Tong;
            DangMuon = response.DangMuon;
            DaTra = response.DaTra;
            QuaHan = response.QuaHan;         

            myPieChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ["Đang mượn", "Đã trả", "Quá hạn"],
                    datasets: [{
                        data: [DangMuon, DaTra, QuaHan],
                        backgroundColor: ['#1cc88a', '#4e73df', '#e74a3b'],
                        hoverBackgroundColor: ['#17a673', '#2e59d9', '#c11808'],
                        hoverBorderColor: "rgba(234, 236, 244, 1)",
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        caretPadding: 10,
                    },
                    legend: {
                        display: false
                    },
                    cutoutPercentage: 80,
                },
            });
        },
        error: function () {
            alert("Đã có lỗi xảy ra.");
        }
    });
})
