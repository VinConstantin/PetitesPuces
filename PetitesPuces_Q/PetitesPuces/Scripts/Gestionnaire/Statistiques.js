$(document).ready(() => {
    const defaultOptions = {
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

    const dayLabels = lastSevenDays();

    const labelsTypesClient = ['Actifs', 'Potentiels', 'Visiteurs'];
    const dataTypesClient = [19, 12, 3];

    const ctxtTotalClients = ctxtForSelector("#cvTotalClients");
    const ctxtTotalVendeurs = ctxtForSelector("#cvTotalVendeurs");
    //const ctxt

    const chartTotalClients = new Chart(ctxtTotalClients, {
        type: 'doughnut',
        data: {
            labels: labelsTypesClient,
            datasets: [
                {
                    label: 'Nb de clients',
                    data: dataTypesClient,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                    ],
                    borderWidth: 1
                }
            ]
        }
    });

    const chartTotalVendeurs = new Chart(ctxtTotalVendeurs, {
        type: 'line',
        data: {
            labels: lastSevenDays(),
            datasets: [
                {
                    label: 'Nombre de vendeurs total',
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
    });

    const chartNouveauxVendeurs = new Chart(ctxtForSelector("#cvNouveauVendeurs"), {
        type: 'line',
        data: {
            labels: lastSevenDays(),
            datasets: [
                {
                    label: 'Demandes de vendeurs',
                    data: [12, 19, 3, 5, 2, 3, 7],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.2)',
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                    ],
                    borderWidth: 1
                },
                {
                    label: 'DemandesAcceptées',
                    data: [12 / 2, 19 / 2, 3 / 2, 5 / 2, 2 / 2, 3 / 2, 7 / 2],
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                    ],
                    borderWidth: 1
                },
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

    const chartNouveauxClients = new Chart(ctxtForSelector("#cvNouveauxClients"), {
        type: 'line',
        data: {
            labels: lastSevenDays(),
            datasets: [
                {
                    label: 'Nouveaux clients',
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
    });

    const chartConnexions = new Chart(ctxtForSelector("#cvConnexionsClients"), {
        type: 'line',
        data: {
            labels: lastSevenDays(),
            datasets: [
                {
                    label: 'Connexions totales',
                    data: [12 * 2, 19 * 2, 3 * 2, 5 * 2, 2 * 2, 3 * 2, 7 * 2],
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                    ],
                    borderWidth: 1
                },
                {
                    label: 'Connexions uniques',
                    data: [12, 19, 3, 5, 2, 3, 7],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.2)',
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                    ],
                    borderWidth: 1
                },
                {
                    label: 'Connexions simultanées',
                    data: [12 / 2, 19 / 2, 3 / 2, 5 / 2, 2 / 2, 3 / 2, 7 / 2],
                    backgroundColor: [
                        'rgba(255, 206, 86, 0.2)',
                    ],
                    borderColor: [
                        'rgba(255, 206, 86, 1)',
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