
var chartColors = {
  red: 'rgb(255, 99, 132)',
  orange: 'rgb(255, 159, 64)',
  yellow: 'rgb(255, 205, 86)',
  green: 'rgb(75, 192, 192)',
  info: '#41B1F9',
  blue: '#3245D1',
  purple: 'rgb(153, 102, 255)',
  grey: '#EBEFF6'
};

//Biểu đồ phiếu mượn
var ctxBar = document.getElementById("bar").getContext("2d");
var BarColor = [
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey,
    chartColors.grey];
var BarData;
$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/Admin/ThongKe/DataBarChart",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (res) {
            BarData = res.CountCallCard;
            if (res.Month > 1) {
                BarColor[res.Month - 1] = chartColors.blue;
                BarColor[res.Month - 2] = chartColors.info;
            } else {
                BarColor[res.Month - 1] = chartColors.blue;
            }
            document.getElementById("AllCallCard").innerHTML = "Tổng số phiếu mượn: " + res.AllCallCard;
            var rate = document.getElementById("RateCallCard");
            if (res.Rate == 0)
                rate.innerHTML = "+ 0%";
            else if (res.Rate > 0)
                rate.innerHTML = "+" + res.Rate + "%";
            else if (res.Rate < 0) {
                rate.innerHTML = res.Rate + "%";
                rate.classList.remove("text-green");
                rate.classList.add("text-red");
            }
            //Create chart
            var myBar = new Chart(ctxBar, {
                type: 'bar',
                data: {
                    labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"],
                    datasets: [{
                        label: 'Phiếu mượn',
                        backgroundColor: BarColor,
                        data: BarData
                    }]
                },
                options: {
                    responsive: true,
                    barRoundness: 1,
                    legend: {
                        display: false
                    },
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true,
                                suggestedMax: 10,
                                padding: 5,
                            },
                            gridLines: {
                                drawBorder: false,
                            }
                        }],
                        xAxes: [{
                            gridLines: {
                                display: false,
                                drawBorder: false
                            }
                        }]
                    }
                }
            });
        }
    })
});
// tỷ lệ phiếu mượn
var ctx = document.getElementById("myPieChart");
$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/Admin/ThongKe/DataPieChart",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (res) {
            //get data
            var data = res.UserData;
            //drawn
            var myPieChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ["Đang hoạt động", "Vi phạm", "Khoá"],
                    datasets: [{
                        data: data,
                        backgroundColor: ['#5a8dee', '#fdac41', '#a3afbd'],
                        hoverBackgroundColor: ['#739ef1', '#fdb85e', '#b1bbc7'],
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
                plugins: [{
                    id: 'text',
                    beforeDraw: function (chart, a, b) {
                        var width = chart.width,
                            height = chart.height,
                            ctx = chart.ctx;
                        ctx.restore();
                        var fontSize = (height / 114).toFixed(2);
                        ctx.font = fontSize + "em sans-serif";
                        ctx.textBaseline = "middle";

                        var text = "Tài khoản",
                            textX = Math.round((width - ctx.measureText(text).width) / 2),
                            textY = height / 2 - 10;
                        ctx.fillText(text, textX, textY);
                        var text = res.Count,
                            textX = Math.round((width - ctx.measureText(text).width) / 2),
                            textY = height / 2;
                        ctx.fillText(text, textX, textY + 35);
                        ctx.save();
                    }
                }]
            });
        }
    });
})


