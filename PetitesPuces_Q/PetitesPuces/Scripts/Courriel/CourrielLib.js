const Courriel = (function() {
    return {
        enregistrerMessage: enregistrerMessage,
        Message: Message,
        redirigerVersMessage: redirigerVersMessage,
        envoyerMessage: envoyerMessage
    }

    /**
     * @param {string} objet
     * @param {string} corps
     * @param {Array<number>} destinataires
     */
    function Message(objet, corps, destinataires) {
        this.DescMsg = corps;
        this.objet = objet;
        this.destinataires = destinataires.slice();
    }

    /**
     * @param {Object} message - Objet Message
     * @param {Function} callback
     */
    function enregistrerMessage(message, callback = null) {
        const method = 'POST';
        const data = JSON.stringify(message);
        console.log((method === 'POST' ? 'Nouveau ' : 'Update ') + 'brouillon');

        $.ajax({
            url: '/Courriel/Enregistrer',
            method: method,
            data: data,
            dataType: 'json',
            processData: false,
            success: (data) => {
                if (callback) callback(data);
            },
            contentType: 'application/json'
        });
    }

    /**
     * @param {Object} courriel - Objet Message
     * @param {Function} callback
     */
    function redirigerVersMessage(courriel, callback = null) {
        enregistrerMessage(courriel, function (data) {
            const noMsg = data.NoMsg;
            const url = "/Courriel/Boite/Composer/" + noMsg;

            window.location.href = url;
        });
    }

    /**
     * @param {Object} courriel - Objet Message
     * @param {Function} callback
     */
    function envoyerMessage(courriel, callback = null) {
        enregistrerMessage(courriel, function (data) {
            const noMsg = data.NoMsg;
            const url = "/Courriel/Envoyer/" + noMsg;

            $.ajax({
                url: url,
                type: "POST",
                data: data,
                success: function (data) {
                    if (callback) callback(data);
                },
                error: function (req, status, err) {
                    console.log(req, status, err);
                }
            });
        });
    }
})();