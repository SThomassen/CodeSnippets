<template>
    <span>
        <l-confirm ref='dialog' @confirm='confirm'></l-confirm>

        <template v-if='preview'>
            <l-preview :blog='blog' :author='author' @close='preview=false'></l-preview>
        </template>
        
        <template  v-else>
            <input 
                ref='upload' 
                type="file" 
                accept="image/*" 
                @change='fileSelect' 
                enctype="multipart/form-data" 
                hidden>
            <v-container fluid>
                <v-card height="320px" width="100%" outlined @click.stop='$refs.upload.click()'>
                    <v-img
                        height="480"
                        :position='`center ${blog.imageOffset}px`'
                        gradient="to top left, rgba(255,255,255,.33), rgba(255,255,255,.4), rgba(50,50,50,.7)"
                        :src='blog.image'>
                        <v-row no-gutters>
                            <v-col cols='6'>
                                <v-list-item :dark='blog.image !== undefined'>
                                    <v-list-item-action-text>
                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon class='mr-3' v-on='on' @click.stop='preview = true'>
                                                    <v-icon>mdi-eye</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t("preview")}}
                                        </v-tooltip>

                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon class='mr-3' v-on='on' @click.stop='save'>
                                                    <v-icon>mdi-content-save</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t("save")}}
                                        </v-tooltip>
                                        
                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon v-on='on' @click.stop='post'>
                                                    <v-icon>mdi-send</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t("submit")}}
                                        </v-tooltip>
                                    </v-list-item-action-text>
                                </v-list-item>
                            </v-col>
                        </v-row>
                        <v-row class='text-center' v-if='blog.image === undefined'>
                            <v-col>
                                <span class='caption'>{{$t('select_image', currentLocale)}}.</span>
                            </v-col>
                        </v-row>
                    </v-img>
                </v-card>
            </v-container>
            <v-container>
                <v-row no-gutters>
                    <v-col cols='12' md='8' offset-md='2'>
                        <v-form ref='blogForm'>
                            <v-list-item>
                                <v-text-field
                                    class='display-1'
                                    :placeholder="$t('blog_title_here', currentLocale)" 
                                    v-model='writeable.title'>
                                </v-text-field>

                                <v-list-item-action>
                                    <v-menu bottom offset-y nudge-left="4" nudge-top="4">
                                        <template v-slot:activator="{ on, attrs }">
                                            <v-btn
                                                icon
                                                v-bind="attrs"
                                                v-on="on">
                                                <v-avatar tile :size='16'>
                                                    <img :src="currentFlag.icon" :alt="currentLocale">
                                                </v-avatar>
                                            </v-btn>
                                        </template>
                                        <v-list>
                                            <v-list-item
                                                v-for="(item, index) in availableLocales"
                                                :key="index"
                                                @click='switchLanguage(item.code)'>
                                                <v-avatar tile :size='16'>
                                                    <img :src="item.icon" :alt="item.name">
                                                </v-avatar>
                                            </v-list-item>
                                        </v-list>
                                    </v-menu>
                                </v-list-item-action>
                            </v-list-item>
                            <!-- <v-divider></v-divider> -->

                            <v-list-item two-line>
                                <v-list-item-avatar>
                                    <l-avatar :builder='author'></l-avatar>
                                </v-list-item-avatar>
                                <v-list-item-content>
                                    <v-list-item-subtitle>
                                        {{author.displayName}}
                                    </v-list-item-subtitle>
                                    <v-list-item-subtitle>
                                        <l-timestamp :date='published' format='short'></l-timestamp><!--D MMMM YYYY-->
                                    </v-list-item-subtitle>
                                </v-list-item-content>
                            </v-list-item>

                            <v-list-item>
                                <v-tabs height="32" v-model='tab'>
                                    <v-tab>
                                        <span class='caption'>Editor</span>
                                    </v-tab>
                                    <v-tab>
                                        <span class='caption'>Html</span>
                                    </v-tab>
                                </v-tabs>
                            </v-list-item>

                            <!-- <v-list-item> -->
                            <v-container>
                                <v-tabs-items v-model='tab'>
                                    <v-tab-item>
                                        <vue-editor id="editor" style='width:100%;' v-model="writeable.message"></vue-editor>
                                    </v-tab-item>

                                    <v-tab-item>
                                        <v-textarea outlined full-width v-model="writeable.message"></v-textarea>
                                    </v-tab-item>
                                </v-tabs-items>
                            </v-container>
                            <!-- </v-list-item> -->

                            <l-tags v-model='blog.tags'></l-tags>

                            <v-list-item>
                                <v-divider></v-divider>
                                <span class='caption'>{{$t('additional_options')}}</span>
                                <v-divider></v-divider>
                            </v-list-item>

                            <v-list-item>
                                <v-list-item-action>
                                    <v-checkbox 
                                        :label='$t("show_at_releases")' 
                                        v-model='stat.releases'>
                                    </v-checkbox>
                                </v-list-item-action>
                                <v-spacer></v-spacer>
                                <v-list-item-action>
                                    <v-select 
                                        item-text='id'
                                        :items='releases' 
                                        v-model='stat.releaseId'>
                                    </v-select>
                                </v-list-item-action>
                            </v-list-item>

                            <v-list-item :hidden='!hasPermission(["set_popup"])'>
                                <v-list-item-action>
                                    <v-checkbox 
                                        :label='$t("messages.show_at_start.title")' 
                                        v-model='stat.popup'>
                                    </v-checkbox>
                                </v-list-item-action>

                                <v-spacer></v-spacer>

                                <v-list-item-action>
                                    <v-btn small outlined @click="previewPopup">
                                        <v-icon left>mdi-eye</v-icon>
                                        {{$t('preview')}}
                                    </v-btn>
                                </v-list-item-action>
                            </v-list-item>

                            <v-list-item dense>
                                <v-list-item-action>
                                    <v-checkbox 
                                        :label='$t("schedule_publish")' 
                                        v-model='stat.schedule'>
                                    </v-checkbox>
                                </v-list-item-action>

                                <v-spacer></v-spacer>

                                <v-list-item-action>
                                    <l-datepicker
                                        :label='$t("publish_date")'
                                        :disabled='!stat.schedule' 
                                        v-model='stat.date'>
                                    </l-datepicker>
                                </v-list-item-action>
                            </v-list-item>
                        </v-form>
                    </v-col>
                </v-row>
            </v-container>
        </template>
    </span>
</template>

<script>
import { VueEditor, Quill } from "vue2-editor";
import Tags from '@/shared/Input/Tags'
import Avatar from '@/shared/Cards/Avatar'
import Timestamp from '@/shared/Input/Timestamp'
import TimePicker from '@/shared/Input/TimePicker'
import DatePicker from '@/shared/Input/DatePicker'

import Confirm from '@/shared/Dialog/Confirm'

import Preview from '@/components/blogs/preview'


export default {
    data() {
        return {
            key: null,
            tab: 0,
            imageOffset: 0,
            preview: false,
            blog: {},
            stat: {},
            today: new Date().getTime(),
            rules: {
                title: [
                    v => !!v || 'Title is required',
                ]
            },
            currentLocale: 'en'
        }
    },
    async created() {
        this.key = await this.$store.dispatch('auth/generateKey');

        this.$bus.$on('editBlog', blog => {
            console.log(blog);
        })
    },
    components: {
        'l-tags': Tags,
        'l-avatar': Avatar,
        'l-timestamp': Timestamp,
        'l-timepicker': TimePicker,
        'l-datepicker': DatePicker,
        'l-preview': Preview,
        'l-confirm': Confirm,
    },
    computed: {
        author() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        published() {
            if (this.stat.schedule && this.stat.date) {
                this.blog.published = new Date(`${this.stat.date}`);
            }
            return (this.blog.published) ? this.blog.published : new Date();
        },
        releases() {
            let releases = this.$store.getters['releases/versions'];
            return releases ? Object.values(releases) : {};
        },
        currentFlag() {
            return this.$i18n.locales.find(item => item.code === this.currentLocale);
        },
        availableLocales () {
            return this.$i18n.locales.filter(i => i.code !== this.currentLocale)
        },
        writeable() {
            if (this.blog[this.currentLocale] === undefined) {
                this.blog[this.currentLocale] = {};
            }
            return this.blog[this.currentLocale];
        }
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ "manage_blog", ...perms ]);
        },
        confirm(func) {
            func();
        },
        previewPopup() {
            this.blog.authorId = this.author.id;
            this.$bus.$emit('onOpenChangelog', this.blog);
        },
        fileSelect(event) {
            const file = event.target.files[0];
            if (!file) return;

            let reader = new FileReader()
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.$set(this.blog, 'image', reader.result);
            }
            this.blog.file = file;
        },
        check() {
            if (this.blog.file === undefined) {
                this.$bus.$emit('onShowMessage', { message: this.$t('no_cover_image'), color: 'error' });
                return false;
            }
            if (this.blog.title === undefined || this.blog.title.length <= 0) {
                this.$bus.$emit('onShowMessage', { message: this.$t('no_title'), color: 'error' });
                return false;
            }
            if (this.blog.message === undefined || this.blog.message.length <= 0) {
                this.$bus.$emit('onShowMessage', { message: this.$t('no_message'), color: 'error' });
                return false;
            }
        },
        async save() {
            try {
                if (this.blog.file) {
                    let src = await this.uploadImage();
                    this.blog.image = src;
                }

                this.blog.id = this.key;
                this.blog.active = false;
                this.blog.authorId = this.author.id;
                this.blog.published = this.published;

                console.log(this.blog);
                
                await this.$store.dispatch('blogs/setBlog', this.blog);
                this.$bus.$emit('onShowMessage', { message: this.$t('blog_saved') });
            } catch(err) {
                console.log(err);
            }
        },
        post() {
            if (!this.check) return;

            if (this.stat.popup) {
                this.$refs.dialog.confirm({ 
                    title: $t("messages.show_at_start.title"), 
                    message: $t("messages.show_at_start.enabled"), 
                    callback: this.setPopup
                });
            } else {
                this.upload();
            }
        },
        async setPopup() {
            try {
                await this.$store.dispatch('blogs/updateStats', {
                    next: this.key
                });
                this.upload();
            } catch(err) {
                console.log(err);
            }
        },
        uploadImage() {
            return new Promise( async (resolve, reject) => {
                try {
                    let file = this.blog.file;
                    var ext = file.name.substr(file.name.lastIndexOf('.') + 1);
                    let fileName = `${this.key}.${ext}`;

                    let upload = await this.$store.dispatch('storage/uploadImage', { 
                        file: file,
                        fileName: fileName,
                        destination: "blogs" 
                    });
                    delete this.blog.file;
                    resolve(upload.src);
                } catch(err) {
                    reject(err);
                }
            })
        },
        async upload() {
            try {
                let src = await this.uploadImage();

                this.blog.id = this.key;
                this.blog.active = true;
                this.blog.image = src;
                this.blog.authorId = this.author.id;
                this.blog.published = this.published;

                await this.$store.dispatch('blogs/setBlog', this.blog);
                if (this.stat.release != undefined) {
                    await this.$store.dispatch('releases/linkBlog', { release: this.stat.release, blog: id });
                }
                this.$router.go(-1);

            } catch(err) {
                console.log(err);
                this.$bus.$emit('onShowMessage', { message: err, color: 'error' });
            }
        },
        switchLanguage(code) {
            this.currentLocale = code;
            console.log(this.currentLocale);
        }
    }
}
</script>