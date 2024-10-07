<template>
     <v-container>
        <v-row class='mt-2'>
            <v-col>
                <span class='text-h2'>{{ $t('contact') }}</span>
                <v-divider></v-divider>

                <v-row>
                    <v-col cols='12' class='pa-2 mt-4'>
                        <v-form ref='contactForm' @submit.prevent='send'>
                            <!-- <v-radio-group v-model="form.type" row :disabled="sending">
                                <v-radio label="Info" value="info"></v-radio>
                                <v-radio label="Bug Report" value="bug report"></v-radio>
                                <v-radio label="Other" value="other"></v-radio>
                            </v-radio-group> -->
                            <v-list>
                                <v-list-item>
                                    <v-text-field
                                        dense
                                        outlined
                                        required
                                        :label='$t("name")'
                                        v-model='form.name'
                                        :disabled="sending"
                                        :rules='rules.nameRules'>
                                        <template v-slot:message='item'>
                                            {{ $t(item.message) }}
                                        </template>
                                    </v-text-field>
                                </v-list-item>

                                <v-list-item>
                                    <v-text-field
                                        dense
                                        outlined
                                        required
                                        :label='$t("email")'
                                        v-model='form.email'
                                        :disabled="sending"
                                        :rules='rules.emailRules'>
                                        <template v-slot:message='item'>
                                            {{ $t(item.message) }}
                                        </template>
                                    </v-text-field>
                                </v-list-item>

                                <v-list-item>
                                    <v-text-field
                                        dense
                                        outlined
                                        required
                                        :label='$t("subject")'
                                        v-model='form.subject'
                                        :disabled="sending"
                                        :rules='rules.subjectRules'>
                                        <template v-slot:message='item'>
                                            {{ $t(item.message) }}
                                        </template>
                                    </v-text-field>
                                </v-list-item>

                                <v-list-item>
                                    <v-textarea
                                        outlined
                                        required
                                        :label='$t("message")'
                                        v-model='form.message'
                                        :disabled="sending"
                                        :rules='rules.messageRules'>
                                        <template v-slot:message='item'>
                                            {{ $t(item.message) }}
                                        </template>
                                    </v-textarea>
                                </v-list-item>

                                <v-list-item-action>
                                    <v-row no-gutters>
                                        <v-btn
                                            outlined
                                            color='info'
                                            type='submit'
                                            :loading='sending'>
                                            {{ $t("send") }}
                                        </v-btn>
                                    </v-row>
                                </v-list-item-action>
                            </v-list>
                        </v-form>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import firebase from '@/plugins/firebase';

export default {
    head() {
        return {
            title: `Layhgobuilder | Contact`,
        }
    },
    data() {
        return {
            sending: false,
            form: {},
            rules: {
                nameRules: [
                    v => !!v || 'rules.name_required',
                    v => (v && v.length <= 20) || 'rules.name_length'
                ],
                emailRules: [
                    v => !!v || 'rules.email_required',
                    v => /.+@.+/.test(v) || 'rules.email_valid'
                ],
                subjectRules: [
                    v => !!v || 'rules.subject_required',
                    v => (v && v.length >= 4) || 'rules.subject_length'
                ],
                messageRules: [
                    v => !!v || 'rules.message_required',
                    v => (v && v.length >= 6) || 'rules.message_length'
                ]
            }
        }
    },
    methods: {
        async send() {
            if (!this.$refs.contactForm.validate()) {
                return;
            }

            this.sending = true;
            const callable = firebase.functions().httpsCallable('contactEmail');
            await callable(this.form);
            this.sending = false;
            this.form = {};
        }
    }
}
</script>