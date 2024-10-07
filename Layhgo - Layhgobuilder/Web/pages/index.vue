<template></template>

<script>
export default {
    head() {
        return {
            title: 'Layhgobuilder',
            meta: [
                { charset: 'utf-8' },
                { name: 'viewport', content: 'width=device-width, initial-scale=1' }
            ],
        }
    },
    methods: {
        verify() {
            
        },
        encrypt() {
            var encrypted = this.$CryptoJS.AES.encrypt("administrator", "Secret Passphrase").toString();
            console.log(encrypted);

            var decrypted = this.$CryptoJS.AES.decrypt(encrypted, "Secret Passphrase");
            var originalText = decrypted.toString(this.$CryptoJS.enc.Utf8);
            console.log(originalText);
        }
    }
}
</script>