<template>
    <v-container fluid>
        <v-form ref='request'>
            <v-row no-gutters class='mt-4'>
                <v-col cols='12'>
                    <span>{{product}}</span>
                </v-col>
                <v-col cols='12'>
                    <v-text-field 
                        dense 
                        outlined 
                        :label='$t("name") + " *"' 
                        :rules='rules.name'
                        v-model='form.name'>
                        <template v-slot:message='item'>
                            {{ $t(item.message) }}
                        </template>
                    </v-text-field>
                </v-col>
                <v-col cols='12'>
                    <v-text-field 
                        dense 
                        outlined
                        :label='$t("email") + " *"'
                        :rules='rules.email'
                        v-model='form.email'>
                        <template v-slot:message='item'>
                            {{ $t(item.message) }}
                        </template>
                    </v-text-field>
                </v-col>
                <v-col cols='12'>
                    <v-text-field 
                        dense 
                        outlined 
                        :label='$t("company")'
                        v-model='form.company'>
                        <template v-slot:message='item'>
                            {{ $t(item.message) }}
                        </template>
                    </v-text-field>
                </v-col>

                <v-col cols='12'>
                    <v-textarea
                        dense
                        outlined
                        :label='$t("reason")'
                        v-model='form.reason'> 
                        <template v-slot:message='item'>
                            {{ $t(item.message) }}
                        </template>
                    </v-textarea>
                </v-col>

                <v-col cols='auto'>
                    <v-btn 
                        outlined 
                        :loading='loading' 
                        @click.prevent='send'>
                        {{$t("send")}}
                    </v-btn>
                </v-col>
            </v-row>
        </v-form>
    </v-container>
</template>

<script>
import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            loading: false,
            form: {},
            rules: {
                name: [
                    v => !!v || 'rules.name_required',
                ],
                email: [
                    v => (!!v || this.type !='email') || 'rules.email_required',
                    v => (/^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(v) || this.type !='email') || 'rules.email_valid'
                ],
            }
        }
    },
    props: {
        product: {
            type: String,
            required: false,
            default: 'Fieldlab'
        }
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return user ? user : {};
        }
    },
    methods: {
        cancel() {
            this.$emit('cancel');
        },
        open() {
            
        },
        async send() {
            if (!this.$refs.request.validate()) {
                return;
            }

            this.loading = true;
            this.form.product = this.product;
            const callable = firebase.functions().httpsCallable('requestAccess');
            await callable(this.form);
            this.loading = false;
            this.cancel();
        }
    }
}
</script>