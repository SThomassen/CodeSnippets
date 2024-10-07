<template>
    <span>
        <l-confirm ref='dialog' @confirm='confirm'></l-confirm>

        <template v-if='preview'>
            <l-preview :blog='wrapper' :author='author' @close='preview=false'></l-preview>
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
                <v-card width="100%" outlined @click.stop='$refs.upload.click()'>
                    <v-img
                        height="480"
                        :position='`center ${wrapper.imageOffset}px`'
                        :src='wrapper.image'>
                        <div class="fill-height" style="position:absolute; width:100%; height:100%; top: 0; left: 0; background-image: linear-gradient(to top left, rgba(255,255,255,.33), rgba(255,255,255,.4), rgba(50,50,50,.7));"></div>
                        <v-row no-gutters class='ml-3 mt-2'>
                            <v-col>
                                <v-list-item :dark='wrapper.image !== undefined'>
                                    <v-list-item-action-text>
                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon class='mr-3' v-on='on' @click.stop='cancel'>
                                                    <v-icon>mdi-close</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t('cancel')}}
                                        </v-tooltip>

                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon class='mr-3' v-on='on' @click.stop='preview = true'>
                                                    <v-icon>mdi-eye</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t('preview')}}
                                        </v-tooltip>

                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon class='mr-3' v-on='on' @click.stop='save'>
                                                    <v-icon>mdi-content-save</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t('save')}}
                                        </v-tooltip>
                                        
                                        <v-tooltip bottom v-if='!wrapper.active'>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon v-on='on' @click.stop='post'>
                                                    <v-icon>mdi-send</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t('submit')}}
                                        </v-tooltip>
                                    </v-list-item-action-text>
                                </v-list-item>
                            </v-col>

                            <v-col cols='auto'>
                                <v-list-item>
                                    <v-list-item-action-text>
                                        <v-text-field dense outlined type="number" v-model="wrapper.imageOffset" @click.prevent="" @blur.prevent="" @focus.prevent=""></v-text-field>
                                    </v-list-item-action-text>
                                </v-list-item>
                            </v-col>
                        </v-row>
                        <v-row class='text-center' v-if='wrapper.image === undefined'>
                            <v-col>
                                <span class='caption'>{{$t('select_image')}}.</span>
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

                            <l-tags v-model='wrapper.tags'></l-tags>

                            <v-list-item>
                                <v-divider></v-divider>
                                <span class='caption'>{{$t('additional_options')}}</span>
                                <v-divider></v-divider>
                            </v-list-item>

                            <v-list-item>
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
                                    <v-menu
                                        offset-y
                                        v-model="menu"
                                        min-width="auto"
                                        transition="scale-transition"
                                        :nudge-right="40"
                                        :close-on-content-click="false">
                                        <template v-slot:activator="{ on, attrs }">
                                            <v-text-field
                                                dense
                                                readonly
                                                outlined
                                                :label='$t("publish_date")'
                                                prepend-icon="mdi-calendar"
                                                v-model="stat.date"
                                                v-bind="attrs"
                                                v-on="on">
                                            </v-text-field>
                                        </template>
                                        <v-date-picker
                                            v-model="stat.date"
                                            @input="menu = false">
                                        </v-date-picker>
                                    </v-menu>
                                    <!-- <l-datepicker
                                        :label='$t("publish_date")'
                                        :disabled='!stat.schedule' 
                                        v-model='stat.date'>
                                    </l-datepicker> -->
                                </v-list-item-action>
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
                                        dense
                                        outlined
                                        item-text='id'
                                        :items='releases' 
                                        v-model='stat.releaseId'>
                                    </v-select>
                                </v-list-item-action>
                            </v-list-item>

                            <v-list-item color='error'>
                                <v-divider></v-divider>
                                <span class='caption'>{{$t('danger_zone')}}</span>
                                <v-divider></v-divider>
                            </v-list-item>

                            <!-- Danger Zone -->
                            <!-- <v-card outlined color='error'> -->
                            <v-list>
                                <v-list-item>
                                    <v-row no-gutters align="center">
                                        <v-btn small outlined color='error' @click="$refs.dialog.confirm({
                                                title: $t('messages.remove_post.title'), 
                                                message: `${$t('messages.remove_post.message')}?`,
                                                callback: remove
                                            });">
                                            <v-icon>mdi-delete</v-icon>
                                            {{$t('delete')}}
                                        </v-btn>
                                    </v-row>
                                </v-list-item>
                            </v-list>
                            <!-- </v-card> -->


                        </v-form>
                    </v-col>
                </v-row>
            </v-container>
        </template>
    </span>
</template>

<script>
import moment from 'moment/moment'
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
            tab: 0,
            menu: false,
            preview: false,
            wrapper: Object.assign ({},this.blog),
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
    async mounted() {
        this.wrapper = Object.assign({}, this.blog);

        await this.getBlog();
        this.setStat();
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
    watch: {
        blog(nv) {
            this.wrapper = Object.assign({}, nv);
            console.log(nv);
        }
    },
    computed: {
        blog() {
            let blog = this.$store.getters['blogs/blogs'][this.$route.query.id];
            return (blog) ? blog : {}
        },
        stats() {
            let stats = this.$store.getters['blogs/stats'];
            return (stats) ? stats : {};
        },
        author() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        published() {
            if (this.stat.schedule && this.stat.date) {
                this.wrapper.published = new Date(`${this.stat.date}`);
            }
            return (this.wrapper.published) ? this.wrapper.published : new Date();
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
            if (this.wrapper[this.currentLocale] === undefined) {
                this.wrapper[this.currentLocale] = { title: this.wrapper.title, message: this.wrapper.message };
            }
            return this.wrapper[this.currentLocale];
        }
    },
    methods: {
        confirm(func) {
            func();
        },
        cancel() {
            this.$router.go(-1);
        },
        previewPopup() {
            this.wrapper.authorId = this.author.id;
            this.$bus.$emit('onOpenChangelog', this.wrapper);
        },
        fileSelect(event) {
            const file = event.target.files[0];
            if (!file) return;

            let reader = new FileReader()
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.$set(this.wrapper, 'image', reader.result);
            }
            this.wrapper.file = file;
        },
        getBlog() {
            return new Promise (async (resolve, reject) => {
                try {
                    if (this.blog.id !== undefined) return;
                    await this.$store.dispatch('blogs/getBlogById', this.$route.query.id);
                    resolve();
                } catch(err) {
                    reject(err);
                }
            })
            
        },
        setStat() {
            this.stat.popup = (this.blog.id === this.stats.current || this.blog.id === this.stats.next);
            if (this.blog.time >= this.today) {
                this.stat.schedule = true;
                this.stat.date = moment(new Date(this.blog.time)).format("DD-MM-YYYY");
            }
        },
        async remove() {
            await this.$store.dispatch('blogs/deleteBlog', this.blog.id);
            this.$router.go(-1);
        },
        check() {
            console.log('check');
            if (this.wrapper.file === undefined && this.wrapper.image === undefined) {
                console.log('onShowMessage no_cover_image');
                this.$bus.$emit('onShowMessage', { message: this.$t('no_cover_image'), color: 'error' });
                return false;
            }
            if (this.wrapper.title === undefined || this.wrapper.title.length <= 0) {
                console.log('onShowMessage no_title');
                this.$bus.$emit('onShowMessage', { message: this.$t('no_title'), color: 'error' });
                return false;
            }
            if (this.wrapper.message === undefined || this.wrapper.message.length <= 0) {
                console.log('onShowMessage no_message');
                this.$bus.$emit('onShowMessage', { message: this.$t('no_message'), color: 'error' });
                return false;
            }
            return true;
        },
        async save() {
           try {
                if (this.wrapper.file) {
                    let src = await this.uploadImage();
                    this.wrapper.image = src;
                    delete this.wrapper.file;
                }
                console.log(this.published);
                this.wrapper.published = this.published;
                
                await this.$store.dispatch('blogs/setBlog', this.wrapper);
                this.$bus.$emit('onShowMessage', { message: 'Blog saved' });
            } catch(err) {
                console.log(err);
            }
        },
        post() {
            console.log('post');
            if (!this.check()) return;

            if (this.stat.popup) {
                console.log('popup');
                this.$refs.dialog.confirm({ 
                    title: $t("show_at_start"), 
                    message: $t("show_at_start_enabled"), 
                    callback: this.setPopup
                });
            } else {
                this.upload();
            }
        },
        async setPopup() {
            try {
                console.log('setPopup');
                await this.$store.dispatch('blogs/updateStats', {
                    next: this.$route.query.id
                });
                this.upload();
            } catch(err) {
                console.log(err);
            }
        },
        uploadImage() {
            return new Promise( async (resolve, reject) => {
                try {
                    let file = this.wrapper.file;
                    var ext = file.name.substr(file.name.lastIndexOf('.') + 1);
                    let fileName = `${this.$route.query.id}.${ext}`;

                    let upload = await this.$store.dispatch('storage/uploadImage', { 
                        file: file,
                        fileName: fileName,
                        destination: "blogs" 
                    });
                    delete this.wrapper.file;
                    resolve(upload.src);
                } catch(err) {
                    reject(err);
                }
            })
        },
        async upload() {
            try {
                console.log('upload');
                let src = await this.uploadImage();
                // let file = this.wrapper.file;
                // var ext = file.name.substr(file.name.lastIndexOf('.') + 1);
                // let fileName = `${this.$route.query.id}.${ext}`;
                
                // let upload = await this.$store.dispatch('storageuploadImage', { 
                //     file: this.wrapper.file,
                //     fileName: fileName,
                //     destination: "blogs" 
                // });
                
                this.wrapper.image = src;
                this.wrapper.authorId = this.author.id;
                this.wrapper.published = this.published;
                // delete this.wrapper.file;
                console.log(this.wrapper);
                let id = await this.$store.dispatch('blogs/setBlog', this.wrapper);
                if (this.stat.release != undefined) {
                    await this.$store.dispatch('releases/linkBlog', { release: this.wrapper.releaseId, blog: id });
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