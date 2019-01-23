$(document).ready(() => {
    const chartOptions = {
        type: 'line',
        data: {
            labels: lastSevenDays(),
            datasets: [
                {
                    label: 'Nb. de demandes',
                    data: [12, 19, 3, 5, 2, 3, 7],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.2)',
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                    ],
                    borderWidth: 1
                }
            ]
        },
        options: {
            scales: {
                yAxes: [
                    {
                        ticks: {
                            beginAtZero: true
                        }
                    }
                ]
            }
        }
    };

    const ctxInactive = ctxtForSelector('#cvInactivite');
    const ctxDemandes = ctxtForSelector('#cvDemandesVendeur');
    const ctxRedevances = ctxtForSelector('#cvRedevances');

    const inactiveGraph = new Chart(ctxInactive, {
        type: 'line',
        data: {
            labels: lastSevenDays(),
            datasets: [
                {
                    label: 'Nb. d\'utilisateurs',
                    data: [12, 19, 3, 5, 2, 3, 7],
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                    ],
                    borderWidth: 1
                }
            ]
        },
        options: {
            scales: {
                yAxes: [
                    {
                        ticks: {
                            beginAtZero: true
                        }
                    }
                ]
            }
        }
    });
    const demandesGraph = new Chart(ctxDemandes, chartOptions);
    const redevancesGraph = new Chart(ctxRedevances, {
        type: 'line',
        data: {
            labels: lastSevenDays(),
            datasets: [
                {
                    label: 'Somme en dollars',
                    data: [12, 19, 3, 5, 2, 3, 7],
                    backgroundColor: [
                        'rgba(133, 187, 101, 0.2)',
                    ],
                    borderColor: [
                        'rgba(133, 187, 101, 1)',
                    ],
                    borderWidth: 1
                }
            ]
        },
        options: {
            scales: {
                yAxes: [
                    {
                        ticks: {
                            beginAtZero: true
                        }
                    }
                ]
            }
        }
    });

    function ctxtForSelector(selector) {
        return $(selector)[0].getContext('2d');
    }

    function lastSevenDays() {
        const week = [];
        const intl = new Intl.RelativeTimeFormat('fr');

        for (let i = -7; i < 0; i++) {
            const formattedDate = intl.format(i, 'day');

            week.push(formattedDate);
        }

        return week;
    }
});