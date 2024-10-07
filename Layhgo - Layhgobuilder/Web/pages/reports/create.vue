<template>
    <v-container>    
        <v-row class='mt-2'>
            <v-col>

                <!-- <v-row no-gutters>
                    <v-col cols='12' class='pa-2 mt-4'> -->
                        <v-tooltip bottom>
                            <template v-slot:activator='{ on }'>
                                <v-btn class='pb-1' icon v-on='on' @click.prevent='back'>
                                    <v-icon>mdi-arrow-left</v-icon>
                                </v-btn>
                            </template>
                            {{$t('back')}}
                        </v-tooltip>

                        <span class='title'>{{ $t('reports.create') }}</span>

                        <!-- </v-col>
                    </v-row> -->
                <v-divider></v-divider>

                <v-row no-gutters>
                    <v-col cols='12' class='pa-2 mt-4'>
                        <v-form ref='reportForm' @submit.prevent='send'>
                            <v-list>
                                <template v-if='user.id !== undefined'>
                                    <v-list-item two-line>
                                        <v-list-item-avatar>
                                            <l-avatar :builder='user'></l-avatar>
                                        </v-list-item-avatar>
                                        <v-list-item-content>
                                            <v-list-item-subtitle>
                                                {{user.displayName}}
                                            </v-list-item-subtitle>
                                            <v-list-item-subtitle>
                                                <l-timestamp :date='published' format='short'></l-timestamp><!--D MMMM YYYY-->
                                            </v-list-item-subtitle>
                                        </v-list-item-content>
                                    </v-list-item>
                                </template>
                                <template v-else>
                                    <v-list-item>
                                        <v-text-field
                                            dense
                                            outlined
                                            required
                                            :label='$t("name")'
                                            v-model='form.authorName'
                                            :disabled="submit"
                                            :rules='rules.nameRules'>
                                        </v-text-field>
                                    </v-list-item>

                                    <v-list-item>
                                        <v-text-field
                                            dense
                                            outlined
                                            required
                                            :label='$t("email")'
                                            v-model='form.authorEmail'
                                            :disabled="submit"
                                            :rules='rules.emailRules'>
                                        </v-text-field>
                                    </v-list-item>
                                </template>

                                <v-list-item>
                                    <v-divider></v-divider>
                                </v-list-item>

                                <v-list-item>
                                    <v-select
                                        dense
                                        outlined
                                        item-value="id"
                                        :items="types"
                                        :label="$t('reports.types.title')"
                                        v-model='form.type'>
                                        <template v-slot:selection='{ item }'>
                                            {{ $t(item.text) }}
                                        </template>
                                        <template v-slot:item='{ item }'>
                                            {{ $t(item.text) }}
                                        </template>
                                    </v-select>
                                </v-list-item>

                                <v-list-item>
                                    <v-select
                                        dense
                                        outlined
                                        item-value="id"
                                        :items="product"
                                        :label="$t('reports.product.title')"
                                        v-model='form.product'>
                                        <template v-slot:selection='{ item }'>
                                            {{ $t(item.text) }}
                                        </template>
                                        <template v-slot:item='{ item }'>
                                            {{ $t(item.text) }}
                                        </template>
                                    </v-select>
                                </v-list-item>
                                
                                <template v-if='show(["bug", "crash"])'>
                                    <v-list-item v-if='form.product !== "web"'>
                                        <v-select
                                            dense
                                            outlined
                                            item-value="id"
                                            item-text='id'
                                            :items="versions"
                                            :label="$t('reports.version.title')"
                                            v-model='form.version'>
                                        </v-select>
                                    </v-list-item>

                                    <v-list-item>
                                        <v-select
                                            dense
                                            outlined
                                            item-value="id"
                                            :items="frequencies"
                                            :label="$t('reports.frequencies.title')"
                                            v-model='form.frequency'>
                                            <template v-slot:selection='{ item }'>
                                                {{ $t(item.text) }}
                                            </template>
                                            <template v-slot:item='{ item }'>
                                                {{ $t(item.text) }}
                                            </template>
                                        </v-select>
                                    </v-list-item>
                                </template>
                                
                                <v-list-item>
                                    <v-divider></v-divider>
                                </v-list-item>


                                <v-list-item>
                                    <v-text-field
                                        dense
                                        outlined
                                        required
                                        :label='$t(`reports.types.content.${this.form.type}.subject`)'
                                        v-model='form.title'
                                        :disabled="submit"
                                        :rules='rules.subjectRules'>
                                    </v-text-field>
                                </v-list-item>

                                <v-list-item v-if='show(["documentation"])'>
                                    <v-text-field
                                        dense
                                        outlined
                                        required
                                        :label='$t(`reports.types.content.${this.form.type}.location`)'
                                        v-model='form.location'
                                        :disabled="submit"
                                        :rules='rules.subjectRules'>
                                    </v-text-field>
                                </v-list-item>

                                <v-list-item>
                                    <v-textarea
                                        dense
                                        outlined
                                        required
                                        :label='$t(`reports.types.content.${this.form.type}.content`)'
                                        v-model='form.content'
                                        :disabled="submit"
                                        :rules='rules.messageRules'>
                                    </v-textarea>
                                </v-list-item>

                                <template v-if='show(["bug", "crash"])'>
                                    <v-list-item>
                                        <v-file-input
                                            dense
                                            chips
                                            outlined
                                            multiple
                                            ref='fileInput'
                                            prepend-icon=""
                                            append-icon="$file"
                                            accept="image/png, image/jpeg, image/bmp"
                                            :label="$t('reports.form.files')"
                                            :rules='rules.fileRules'
                                            :show-size="1000"
                                            v-model='files'
                                            @click:append.prevent='$refs.fileInput.$refs.input.click()'>
                                        </v-file-input>
                                    </v-list-item>
                                </template>

                                <v-list-item v-if='show(["bug", "crash"])'>
                                    <v-list-item-content>
                                        <v-data-table 
                                            hide-default-footer
                                            :headers='headers'
                                            :items='steps'>
                                            <template v-slot:top>
                                                <v-row no-gutters>
                                                    <v-col cols='stretch'>
                                                        <v-text-field
                                                            dense
                                                            outlined
                                                            required
                                                            :label='$t(`reports.form.steps`)'
                                                            :disabled="submit"
                                                            v-model='step'
                                                            @submit="pushStep()">
                                                        </v-text-field>
                                                    </v-col>
                                                    <v-col cols='auto'>
                                                        <v-btn class='mb-7 ml-2' @click='pushStep()'>
                                                            +
                                                        </v-btn>
                                                    </v-col>
                                                </v-row>
                                            </template>

                                            <template v-slot:header.step='{item}'>
                                                {{ $t('reports.steps' )}}
                                            </template>

                                            <template v-slot:item.nr='{ index }'>
                                                {{ index + 1 }}
                                            </template>

                                            <template v-slot:item.action='{ index }'>
                                                <v-btn icon @click="removeStep(index)">X</v-btn>    
                                            </template>
                                        </v-data-table>
                                    </v-list-item-content>
                                </v-list-item>

                                <v-list-item-action>
                                    <v-row no-gutters>
                                        <v-btn
                                            outlined
                                            color='info'
                                            type='submit'
                                            :loading='submit'>
                                            {{ $t("submit") }}
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
import Tags from '@/shared/Input/Tags'
import Avatar from '@/shared/Cards/Avatar'
import Timestamp from '@/shared/Input/Timestamp'

import info from '@/config/report'
import firebase from '@/plugins/firebase';

export default {
    head() {
        return {
            title: `Layhgobuilder | Reports`,
        }
    },
    data() {
        return {
            step: "",
            steps: [],
            files: [],
            submit: false,
            form: {
                type: 'bug',
            },
            rules: {
                nameRules: [
                    v => !!v || this.$t('rules.name_required'),
                    v => (v && v.length <= 20) || this.$t('rules.name_length')
                ],
                emailRules: [
                    v => !!v || this.$t('rules.email_required'),
                    v => /.+@.+/.test(v) || this.$t('rules.email_valid')
                ],
                subjectRules: [
                    v => !!v || this.$t('rules.subject_required'),
                    v => (v && v.length >= 4) || this.$t('rules.subject_length')
                ],
                messageRules: [
                    v => !!v || this.$t('rules.message_required'),
                    v => (v && v.length >= 6) || this.$t('rules.message_length')
                ],
                fileRules: [
                    v => !!v || (v && this.files.length < 4) || this.$t('rules.file_length', { length: 4 }),
                    v => !!v || (v && v.size  < 2000000) || this.$t('rules.file_size', { size: '2mb' })
                ]
            },
            headers: [
                {
                    text: 'Nr.',
                    align: 'start',
                    sortable: false,
                    value: 'nr',
                },
                { 
                    text: 'step', 
                    value: 'step',
                    align: 'start',
                    sortable: false
                },
                { 
                    text: '', 
                    value: 'action',
                    sortable: false
                },
            ],
        }
    },
    created() {
        this.getStats();
    },
    components: {
        'l-tags': Tags,
        'l-avatar': Avatar,
        'l-timestamp': Timestamp,
    },
    computed: {
        stats() {
            let stats = this.$store.getters['reports/stats'];
            return stats ? stats : {};
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return user ? user : {};
        },
        published() {
            return new Date();
        },
        versions() {
            let versions = Object.values(this.$store.getters['releases/versions']).filter(release => release.product === this.form.product);
            if (versions.length > 0) {
                versions.push( `${ this.$t('reports.version.older').toLowerCase() }` );
            }
            return versions;
        },
        product() {
            return info.product;
        },
        frequencies() {
            return info.frequencies;
        },
        types() {
            return info.types;
        }
    },
    methods: {
        back() {
            this.$router.go(-1);
        },
        show(state) {
            return state.includes(this.form.type);
        },
        pushStep() {
            this.steps.push({ step: this.step });
            this.form.steps = this.steps;
            this.step = "";
        },
        removeStep(i) {
            this.steps.splice(i,1);
            this.form.steps = this.steps;
        },
        async send() {
            if (!this.$refs.reportForm.validate()) {
                return;
            }
            this.submit = true;
            let id = await this.$store.dispatch('auth/generateKey');

            let files = [];
            this.files.forEach( file => {
                const reader = new FileReader();
                reader.addEventListener("load", () => {}, false);
                reader.readAsDataURL(file);

                file.dest = `reports/${id}`;
                files.push({ dest: `reports/${id}`, name: file.name, data: file });
            });

            if (files.length > 0) {
                this.form.images = await this.$store.dispatch('storage/uploadFiles', { files: files });
            }

            this.form.id = id;
            if (this.user.id !== undefined) {    
                this.form.authorId = this.user.id;
                this.form.authorName = this.user.displayName;
                this.form.authorEmail = this.user.email;
            }

            await this.$store.dispatch('reports/setReport', this.form);

            const absoluteURL = new URL(`${this.form.title}?id=${this.form.id}`, window.location.href).href;
            const callable = firebase.functions().httpsCallable('createReportEmail');
            await callable({
                name: this.form.authorName,
                email: this.form.authorEmail,
                type: this.form.type,
                title: this.form.title,
                content: this.form.content,
                reportURL: absoluteURL
            });

            this.submit = false;
            this.$router.go(-1);
        },
        async getStats() {
            if (this.stats.total === undefined) {
                this.$store.dispatch('reports/getStats');
            }
            if (this.versions === undefined || this.versions.length <= 1) {
                this.$store.dispatch('releases/getReleases', { query: null, limit: 10 });
            }
        }
    }
}
</script>