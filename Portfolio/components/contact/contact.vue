<template>
    <v-container fluid>
        <v-row justify="space-around" no-gutters>
            <v-col cols="auto">
                <v-list-item>
                    <v-list-item-icon>
                        <v-icon right>mdi-map-marker</v-icon>
                    </v-list-item-icon>
                    <v-list-item-title>
                        <span>Breda</span>
                    </v-list-item-title>
                </v-list-item>
            </v-col>
            <!-- <v-col cols="auto">
                <v-list-item>
                    <v-list-item-icon>
                        <v-icon right>mdi-email</v-icon>
                    </v-list-item-icon>
                    <v-list-item-title>
                        <span>Sjors2505@gmail.com</span>
                    </v-list-item-title>
                </v-list-item>
            </v-col> -->
            <v-col cols="auto" v-for="link in links" :key="link.title">
                <v-list-item @click="open(link.url)">
                    <v-list-item-icon>
                        <v-icon right>{{link.icon}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-title>
                        <span>{{$t(link.title)}}</span>
                    </v-list-item-title>
                </v-list-item>
            </v-col>
        </v-row>
        <!-- <v-row no-gutters>
            <v-col cols="6">
                <v-list dense>
                    <v-list-item>
                        <v-list-item-icon>
                            <v-icon>mdi-map-marker</v-icon>
                        </v-list-item-icon>
                        <v-list-item-title>
                            <span>Breda</span>
                        </v-list-item-title>
                    </v-list-item>
                    <v-list-item>
                        <v-list-item-icon>
                            <v-icon>mdi-email</v-icon>
                        </v-list-item-icon>
                        <v-list-item-title>
                            <span>Sjors2505@gmail.com</span>
                        </v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-col>
            <v-col>
                <v-row justify="end" no-gutters>
                    <v-col cols="auto">
                        <v-list dense>
                            <v-list-item v-for="link in links" :key="link">
                                <v-list-item-icon>
                                    <v-icon>{{link.icon}}</v-icon>
                                </v-list-item-icon>
                                <v-list-item-title>
                                    <span>{{$t(link.title)}}</span>
                                </v-list-item-title>
                            </v-list-item>
                        </v-list>
                    </v-col>
                </v-row>
            </v-col>
        </v-row> -->

        
        <!-- <v-form ref='contactForm' @submit.prevent='send'>
            <v-row>
                <v-col lg="6" cols="12">
                    <v-list-item>
                        <v-text-field
                            dense
                            outlined
                            required
                            prepend-icon="mdi-account"
                            v-model='form.name'
                            :label='$t("name")'
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
                            prepend-icon="mdi-at"
                            v-model='form.email'
                            :label='$t("email")'
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
                            prepend-icon="mdi-message"
                            :label='$t("subject")'
                            v-model='form.subject'
                            :disabled="sending"
                            :rules='rules.subjectRules'>
                            <template v-slot:message='item'>
                                {{ $t(item.message) }}
                            </template>
                        </v-text-field>
                    </v-list-item>
                </v-col>
                <v-col lg="6" cols="12">
                    <v-list-item>
                        <v-textarea
                            outlined
                            required
                            rows="6"
                            prepend-icon="mdi-email"
                            :label='$t("message")'
                            v-model='form.message'
                            :disabled="sending"
                            :rules='rules.messageRules'>
                            <template v-slot:message='item'>
                                {{ $t(item.message) }}
                            </template>
                        </v-textarea>
                    </v-list-item>

                    <v-list-item>
                        <v-spacer></v-spacer>
                        <v-list-item-action-text>
                            <v-btn
                                outlined
                                color='info'
                                type='submit'
                                class="ml-8"
                                :loading='sending'>
                                {{ $t("send_message") }}
                            </v-btn>
                        </v-list-item-action-text>
                    </v-list-item>
                </v-col>
            </v-row>
        </v-form> -->
    </v-container>
</template>

<script>
    export default {
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
                },
                links: [
                    {
                        title: "email",
                        url: "mailto:sjors2505@gmail.com",
                        icon: "mdi-email"
                    },
                    {
                        title: 'linkedin',
                        url: 'https://linkedin.com/in/sjors-thomassen/',
                        icon: 'mdi-linkedin'
                    },
                    {
                        title: 'github',
                        url: 'https://github.com/SThomassen',
                        icon: 'mdi-github'
                    }
                ],
            }
        },
        methods: {
            async send() {
                if (!this.$refs.contactForm.validate()) {
                    return;
                }
    
                this.sending = true;

                this.$mail.send({
                    name: this.form.name,
                    from: this.form.email,
                    subject: this.form.subject,
                    text: this.form.message,
                })

                // const callable = firebase.functions().httpsCallable('contactEmail');
                // await callable(this.form);
                this.sending = false;
                this.form = {};
            },
            open(url) {
                window.open(url, '_blank');
            }
        }
    }
</script>