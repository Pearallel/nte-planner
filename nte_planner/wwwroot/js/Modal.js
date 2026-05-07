window.bootstrapInterop = {
    instances: {}, // Dictionary to hold our modal instances

    showModal: function (selector) {
        var el = document.querySelector(selector);
        if (!el) return;

        var id = el.id;

        // If we haven't initialized this modal yet, create it and save it.
        // If it already exists, we skip this and just reuse the existing one.
        if (!this.instances[id]) {
            this.instances[id] = new bootstrap.Modal(el);
        }

        // Tell the instance to show itself
        this.instances[id].show();
    },

    hideModal: function (selector) {
        var el = document.querySelector(selector);
        if (!el) return;

        var id = el.id;

        // Retrieve the saved instance and hide it
        if (this.instances[id]) {
            this.instances[id].hide();
        }
    }
};