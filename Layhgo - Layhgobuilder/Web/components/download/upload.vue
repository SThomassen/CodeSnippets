<template>
    <v-dialog 
        v-model='dialog'
        max-width='640px'
        transition="dialog-bottom-transition"
        @keydown.esc="cancel"
        @click:outside='cancel' >
        <v-card>
            <v-app-bar flat>
                <v-btn icon @click='cancel'>
                    <v-icon>mdi-close</v-icon>
                </v-btn>

                <v-toolbar-title>{{$t('releases.upload')}}</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-btn outlined :loading='submit' @click.prevent='upload()' color='info'>{{$t('upload')}}</v-btn>
            </v-app-bar>

            <v-form ref='uploadForm'>
                <v-container>
                    <v-row>
                        <v-col>
                            <v-text-field
                                dense
                                outlined
                                type='number'
                                :label='$t("reports.version.title")'
                                :min='new Date().getFullYear()'
                                :max='new Date().getFullYear() + 2'
                                v-model="version.year">
                            </v-text-field>
                        </v-col>
                        <v-col>
                            <v-text-field
                                dense
                                outlined
                                type='number'
                                :min='1'
                                :max='4'
                                v-model="version.quarter">
                            </v-text-field>
                        </v-col>
                        <v-col>
                            <v-text-field
                                dense
                                outlined
                                type='number'
                                :min='0'
                                :max='999'
                                v-model="version.iteration">
                            </v-text-field>
                        </v-col>
                        <v-col cols='3'>
                            <v-select
                                dense
                                outlined
                                item-value='id'
                                v-model='version.type'
                                :items='publish'>
                                <template v-slot:selection='{ item }'>
                                    {{ $t(item.text) }}
                                </template>
                                <template v-slot:item='{ item }'>
                                    {{ $t(item.text) }}
                                </template>
                            </v-select>
                        </v-col>
                        <v-col cols='3'>
                            <v-select
                                dense
                                outlined
                                item-value="id"
                                v-model='version.product'
                                :items='product'>
                                <template v-slot:selection='{ item }'>
                                    {{ $t(item.text) }}
                                </template>
                                <template v-slot:item='{ item }'>
                                    {{ $t(item.text) }}
                                </template>
                            </v-select>
                        </v-col>
                    </v-row>

                    <v-row no-gutters v-if='version.product !== "web"'>
                        <v-col cols='12'>
                            <l-file 
                                ref='win' 
                                append-icon=""
                                label="releases.files.windows"
                                v-model='files.win'
                                @completed='done'>
                            </l-file>
                        </v-col>
                        <v-col cols='12'>
                            <l-file 
                                ref='mac' 
                                label="releases.files.mac"
                                v-model='files.mac'
                                @completed='done'>
                            </l-file>
                        </v-col>
                    </v-row>
                </v-container>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
import FilePicker from '@/shared/Input/FilePicker'
import info from '@/config/report'

export default {
    data() {
        return {
            menu: false,
            dialog: false,
            submit: false,
            files: {},
            uploading: {},
            date: (new Date(Date.now() - (new Date()).getTimezoneOffset() * 60000)).toISOString().substr(0, 10),
            version: {
                year: new Date().getFullYear(),
                quarter: Math.floor((new Date().getMonth() + 3) / 3),
                iteration: 0,
                type: "official",
                product: "pro"
            },
        }
    },
    components: {
        'l-file': FilePicker
    },
    computed: {
        product() {
            return info.product;
        },
        publish() {
            return info.publish;
        },
        versionFull() {
            let product = Object.values(this.product).map(el => el.id).indexOf(this.version.product);
            return `${this.version.year}.${product}.${this.version.quarter}.${this.version.iteration}${this.version.type === "beta" ? '.b' : ''}`;
        },
        destination() {
            return `/releases/${this.version.product}/${this.version.type}`;
        }
    },
    methods: {
        open() {
            this.dialog = true;
        },  
        cancel() {
            this.uploading = {};
            this.files = {};

            this.submit = false;
            this.dialog = false;
        },
        setDate() {
            let date = new Date(this.date);
            this.version.year = date.getFullYear(),
            this.version.quarter = Math.floor((date.getMonth() + 3) / 3);
            this.menu = false;
        },
        async upload() {
            if (this.version.product !== 'web' && this.files.mac === undefined && this.files.win === undefined) {
                console.log('files empty');
                return;
            }

            this.submit = true;
            let files = Object.entries(this.files);
            if (files.length <= 0) {
                let file = {
                    version: this.versionFull,
                    type: this.version.type,
                    product: this.version.product,
                    os: 'empty'
                };
                this.uploading['empty'] = file;
                this.done(file);
                return;
            }

            files.forEach(([key, value]) => {
                let file = {
                    name: `Layhgobuilder_${key}.zip`,
                    dest: this.destination,
                    version: this.versionFull,
                    type: this.version.type,
                    product: this.version.product,
                    platform: key,
                    data: value,
                    completed: false
                };
                this.uploading[key] = file;
                this.$refs[key].upload(file);
            });
        },
        async done(file) {
            console.log(file);
            this.uploading[file.platform].completed = true;
            if (Object.values(this.uploading).filter(value => !value.completed).length <= 0) {
                console.log('upload finished');

                await this.$store.dispatch('releases/setRelease', {
                    id: this.versionFull,
                    destination: this.destination,
                    type: this.version.type,
                    product: this.version.product,
                });

                this.cancel();
            }
        }
    }
}
</script>