$(document).ready(() => {
    const BLEU_TRANSPARENT = 'rgba(54, 162, 235, 0.2)';
    const BLEU = 'rgba(54, 162, 235, 1)';
    const ROUGE_TRANSPARENT = 'rgba(255, 99, 132, 0.2)';
    const ROUGE = 'rgba(255, 99, 132, 1)';
    const VERT_TRANSPARENT = 'rgba(133, 187, 101, 0.2)';
    const VERT = 'rgba(133, 187, 101, 1)';

    const DATA_URL = '/Gestionnaire/DonneesIndex';

    const OPTIONS_DEFAUT = {
        scales: {
            yAxes: [
                {
                    ticks: {
                        beginAtZero: true
                    }
                }
            ]
        }   
    };

    const ctxInactive = ctxtForSelector('#cvInactivite');
    const ctxDemandes = ctxtForSelector('#cvDemandesVendeur');
    const ctxRedevances = ctxtForSelector('#cvRedevances');

    $.post(DATA_URL, null, creerGraphiques);

    function creerGraphiques(data) {
        creerGraphiqueDemandes(data.NombreDemandesVendeur);
        creerGraphiqueRedevances(data.Redevances);
        creerGraphiqueInactivite(data.UtilisateursInactifs);
    }

    function creerGraphiqueDemandes(demandes) {
        const dataSet = creerDataSet(demandes, 'Nb. de demandes');
        ajouterCouleursADataset(dataSet, BLEU_TRANSPARENT, BLEU);

        new Chart(ctxDemandes, {
            type: 'line',
            data: dataSet,
            options: OPTIONS_DEFAUT,
        });
    }

    function creerGraphiqueRedevances(redevances) {
        const dataSet = creerDataSet(redevances, 'Redevances en $');
        ajouterCouleursADataset(dataSet, VERT_TRANSPARENT, VERT);

        new Chart(ctxRedevances, {
            type: 'line',
            data: dataSet,
            options: OPTIONS_DEFAUT,
        });
    }

    function creerGraphiqueInactivite(utilisateursInactifs) {
        const dataSet = creerDataSet(utilisateursInactifs, 'Nb. d\'utilisateurs inactifs');
        ajouterCouleursADataset(dataSet, ROUGE_TRANSPARENT, ROUGE);

        new Chart(ctxInactive, {
            type: 'line',
            data: dataSet,
            options: OPTIONS_DEFAUT,
        });
    }

    function creerDataSet(demandes, etiquetteDonnees) {
        const etiquettes = [];
        const donnees = [];
        const intl = new Intl.RelativeTimeFormat('fr');

        const today = new Date();
        const ticksInDay = 1000 * 60 * 60 * 24;

        for (let dateStr in demandes) {
            const dateTicks = Date.parse(dateStr);
            const timeDiff = dateTicks - today.getTime();
            const formattedDate = intl.format(Math.round(timeDiff / ticksInDay, 0), 'day');

            etiquettes.push(formattedDate);
            donnees.push(demandes[dateStr]);
        }

        return {
            labels: etiquettes,
            datasets: [
                {
                    label: etiquetteDonnees,
                    data: donnees,
                }
            ]
        }
    }

    function ajouterCouleursADataset(dataset, couleurFill, couleurBordure) {
        const dataSet = dataset.datasets[0];

        dataSet.backgroundColor = [couleurFill];
        dataSet.borderColor = [couleurBordure];
        dataSet.borderWidth = 1;
    }

    function ctxtForSelector(selector) {
        return $(selector)[0].getContext('2d');
    }
});