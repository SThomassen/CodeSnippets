<template>
    <v-container>
        <l-confirm ref='dialog' @confirm='confirm'></l-confirm>
        <v-row class='mt-2'>
            <v-col>

                <v-row justify="center">
                    <v-col cols='10'>
                        <v-tooltip bottom>
                            <template v-slot:activator='{ on }'>
                                <v-btn class='mb-3' icon v-on='on' :to='localePath("/reports")'>
                                    <v-icon>mdi-arrow-left</v-icon>
                                </v-btn>
                            </template>
                            {{$t('back')}}
                        </v-tooltip>

                        <span class='display-1'>{{wrapper.title}}</span>
                        <v-chip dark class='ml-2 mb-3' :color='typeColor'>{{ $t(`reports.types.content.${wrapper.type}.label`) }}</v-chip>
                    </v-col>
                    <v-spacer></v-spacer>   

                    <v-col cols='auto'>
                        <v-tooltip bottom>
                            <template v-slot:activator='{ on }'>
                                <v-btn class='mb-3' icon v-on='on' @click.prevent='follow'>
                                    <v-icon>mdi-bell-outline</v-icon>
                                </v-btn>
                            </template>
                            {{$t('subscribe')}}
                        </v-tooltip>
                    </v-col>
                </v-row>

                <v-divider></v-divider>

                <v-list-item two-line>
                    <v-list-item-avatar>
                        <l-avatar :builder='author'></l-avatar>
                    </v-list-item-avatar>
                    <v-list-item-content>
                        <v-list-item-subtitle>
                            {{wrapper.authorName}}
                        </v-list-item-subtitle>
                        <v-list-item-subtitle>
                            <l-timestamp :date='wrapper.created' format='short'></l-timestamp><!--D MMMM YYYY-->
                        </v-list-item-subtitle>
                    </v-list-item-content>
                </v-list-item>

                <v-list-item v-if='wrapper.location !== undefined'>
                    <span class='caption'>Location: </span>
                    <span class='font-weight-bold ml-2'>{{wrapper.location}}</span>
                </v-list-item>

                <v-list-item>
                    <v-list-item-content>
                        <!-- <v-card class='pa-4' tile outlined> -->
                            <span v-html='wrapper.content'></span>
                        <!-- </v-card>           -->
                    </v-list-item-content>
                </v-list-item>

                <v-list-item v-if='wrapper.steps !== undefined'>
                    <v-list-item-content>
                        <v-data-table 
                            hide-default-footer
                            :headers='headers'
                            :items='wrapper.steps'>
                            <template v-slot:header.step='{item}'>
                                {{ $t('reports.steps' )}}
                            </template>
                            <template v-slot:item.nr='{ index }'>
                                {{ index + 1 }}
                            </template>
                        </v-data-table>
                    </v-list-item-content>
                </v-list-item>
                

                <v-list-item class='mt-4'>
                    <v-list-item-action-text>
                        <span class='mr-2'>{{ $t('reports.product.title') }}: </span>
                        <v-chip :color='productColor' dark>
                            {{ wrapper.product }}
                        </v-chip>
                    </v-list-item-action-text>
                    <v-list-item-action-text class='ml-4' v-if='wrapper.version !== undefined'>
                        <span class='mr-2'>{{ $t('reports.version.title') }}: </span>
                        <v-chip color='primary' dark>
                            {{ wrapper.version }}
                        </v-chip>
                    </v-list-item-action-text>
                    <v-list-item-action-text class='ml-4'>
                        <span class='mr-2'>{{ $t('reports.states.title') }}: </span>
                        <v-chip :color='stateColor' dark>
                            {{ $t(`reports.states.content.${wrapper.state}`) }}
                        </v-chip>
                    </v-list-item-action-text>
                    <v-list-item-action-text class='ml-4' v-if='wrapper.frequency !== undefined'>
                        <span class='mr-2'>{{ $t('reports.frequencies.title') }}: </span>
                        <v-chip :color='frequencyColor' dark>
                            {{ $t(`reports.frequencies.content.${wrapper.frequency}`) }}
                        </v-chip>
                    </v-list-item-action-text>

                    <v-spacer></v-spacer>

                    <v-list-item-action-text>
                        <span>{{$t("reports.report")}}: #{{wrapper.number}}</span>
                    </v-list-item-action-text>
                </v-list-item>

                <v-list-item v-if='wrapper.images !== undefined && wrapper.images.length > 0'>
                    <v-list-item-content>
                        <l-slideshow small :images='slideshow'></l-slideshow>
                    </v-list-item-content>
                </v-list-item>

                <v-divider></v-divider>

                <v-timeline dense v-if='comments !== undefined && comments.length > 0'>
                    <v-timeline-item  
                        right
                        small
                        v-for='comment in comments'
                        :key='comment.id'>
                        <l-comment :comment='comment'></l-comment>
                    </v-timeline-item>
                </v-timeline>

                <l-comments type='reports' :target='wrapper' :requireUser='false'></l-comments>
            </v-col>

            <v-col md='2' class='pl-6' v-if='hasPermission(["modify_post"])'>
                <v-card>
                    <v-list>
                        <v-list-item>
                            <span>{{$t("settings")}}</span>
                            <v-spacer></v-spacer>
                            <v-btn icon v-if='changes' @click.prevent="updateChanges">
                                <v-icon>mdi-content-save</v-icon>
                            </v-btn>
                        </v-list-item>
                        <v-divider></v-divider>
                        <v-list-item class='mt-4'>
                            <v-select 
                                chips 
                                dense
                                item-value='id'
                                :items='product'
                                :label='$t("reports.product.title")'
                                v-model='wrapper.product'>
                                <template v-slot:selection='{ item }'>
                                    <v-chip :color='item.color' dark>
                                        {{ $t(item.text) }}
                                    </v-chip>
                                </template>
                                <template v-slot:item='{ item }'>
                                    {{ $t(item.text) }}
                                </template>
                            </v-select>
                        </v-list-item>

                        <v-list-item class='mt-4'>
                            <v-select 
                                chips 
                                dense
                                item-value='id'
                                :items='types'
                                :label='$t("reports.types.title")'
                                v-model='wrapper.type'>
                                <template v-slot:selection='{ item }'>
                                    <v-chip :color='item.color' dark>
                                        {{ $t(item.text) }}
                                    </v-chip>
                                </template>
                                <template v-slot:item='{ item }'>
                                    {{ $t(item.text) }}
                                </template>
                            </v-select>
                        </v-list-item>

                        <v-list-item class='mt-4'>
                            <v-select 
                                chips 
                                dense
                                item-value='id'
                                :items='states'
                                :label='$t("reports.states.title")'
                                v-model='wrapper.state'>
                                <template v-slot:selection='{ item }'>
                                    <v-chip :color='item.color' dark>
                                        {{ $t(item.text) }}
                                    </v-chip>
                                </template>
                                <template v-slot:item='{ item }'>
                                    {{ $t(item.text) }}
                                </template>
                            </v-select>
                        </v-list-item>

                        <v-list-item class='mt-4' v-if='report.state !== "fixed" && wrapper.state === "fixed" && wrapper.product === "enterprise"'>
                            <v-select
                                dense
                                outlined
                                item-value='id'
                                item-text='id'
                                :items="versions"
                                :label="$t('reports.states.content.fixed')"
                                v-model='wrapper.fix'>
                            </v-select>
                        </v-list-item>

                        <v-list-item class='mt-4' v-if='report.state !== "duplicate" && wrapper.state === "duplicate"'>
                            <v-text-field 
                                dense 
                                outlined 
                                clearable 
                                v-model='duplicate' 
                                append-icon="mdi-arrow-right" 
                                @click:append.prevent='$refs.duplicateDialog.open()'>
                            </v-text-field>
                            <l-duplicate ref='duplicateDialog' :report='wrapper' @onSelect='selectDuplicate'></l-duplicate>
                        </v-list-item>

                        <v-list-item class='mt-4' v-if='report.state !== "wont_fix" && wrapper.state === "wont_fix"'>
                            <v-textarea 
                                dense
                                outlined
                                :label="$t('reports.form.reason_denied')"   
                                v-model='wrapper.reason'>
                            </v-textarea>
                        </v-list-item>

                        <v-list-item>
                            <v-btn 
                                dense
                                block
                                outlined
                                color='error'
                                @click.prevent='$refs.dialog.confirm({ 
                                    title: $t("messages.remove_report.title"), 
                                    message: `${$t("messages.remove_report.message")}?`, 
                                    callback: remove
                                })'>
                                Delete
                                <v-icon>mdi-delete</v-icon>
                            </v-btn>
                        </v-list-item>
                    </v-list>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Timestamp from '@/shared/Input/Timestamp'
import Comments from '@/components/comments/comments'
import Comment from '@/components/comments/comment'
import Duplicate from '@/components/reports/duplicate'
import Slideshow from '@/shared/Cards/Slideshow'
import Confirm from '@/shared/Dialog/Confirm'

import info from '@/config/report'

export default {
    head () {
        return {
            title: `Layhgobuilder | ${this.report.title}`,
        }
    },
    data() {
        return {
            wrapper: {
                type: "bug",
                state: "open",
                frequency: "once",
                product: "enterprise"
            },
            duplicate: "",
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
                    sortable: false }
            ],
        }
    },
    mounted() { 
        this.wrapper = Object.assign({}, this.report);
        this.$nextTick( async () => {
            try {
                await this.getReport();
                await this.getAuthor();
                this.wrapper = Object.assign({}, this.report);
            } catch(err) {
                console.log(err);
            }
        })
    },
    components: {
        'l-avatar': Avatar,
        'l-timestamp': Timestamp,
        'l-comments': Comments,
        'l-comment': Comment,
        'l-duplicate': Duplicate,
        'l-slideshow': Slideshow,
        'l-confirm': Confirm,
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        builders() {
            return this.$store.getters['auth/builders'];
        },
        author() {
            let author = Object.values(this.builders).find(builder => { return builder.id === this.report.authorId; });
            return (author) ? author : {};
        },
        report() {
            let report = this.$store.getters['reports/reports'][this.$route.query.id];
            return (report) ? report : {};
        },
        comments() {
            let comments = this.$store.getters['comments/comments'];
            return Object.values(comments).filter( item => { return item.type === "report" && item.source === this.report.id }).sort(function(a,b) { return a.time - b.time; });
        },
        changes() {
            let reference = JSON.stringify(this.wrapper);
            let report = JSON.stringify(this.report);

            if (this.wrapper.id === undefined) return false;
            return reference !== report;
        },
        types() {
            return info.types;
        },
        states() {
            return info.states;
        },
        versions() {
            let releases = this.$store.getters['releases/versions'];
            return releases ? Object.values(releases) : []; // info.versions;
        },
        product() {
            return info.product;
        },
        typeColor() {
            let type = info.types.find(item => item.id === this.wrapper.type);
            return type?.color;
        },
        stateColor() {
            let state = info.states.find(item => item.id === this.wrapper.state);
            return state?.color;
        },
        frequencyColor() {
            let frequency = info.frequencies.find(item => item.id === this.wrapper.frequency);
            return frequency?.color;
        },
        productColor() {
            let product = info.product.find(item => item.id === this.wrapper.product);
            return product?.color;
        },
        slideshow() {
            if (this.report.images === undefined) return [];
            return Object.values(this.report.images);
        }
    },
    methods: {
        confirm(func) {
            func();
        },
        subscribe() {
            
        },
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ "manage_blog", ...perms ]);
        },
        show(state) {
            return state.includes(this.form.type);
        },
        follow() {
            
        },
        selectDuplicate(item) {
            this.wrapper.duplicate = item;
            this.duplicate = `#${item.number} ${item.title}`;
        },
        async updateChanges() {
            let message = "";
            let url = "";
            if (this.wrapper.state !== this.report.state) {
                let stateName = this.$t(`reports.states.content.${this.wrapper.state}`, 'en');
                switch(this.wrapper.state) {
                    case "duplicate":
                        let title = this.wrapper.duplicate.title.replace(/[^a-zA-Z0-9]/g,"_");
                        let id = this.wrapper.duplicate.id;

                        url = this.localePath({ name: "reports-reportName", params: { reportName: title }, query: { id: id } })
                        message += this.$t('reports.changes.duplicate', 'en', { number: this.wrapper.duplicate.number, url: url});
                        break;
                    case "fixed":
                        if (this.wrapper.product === "enterprise") {
                            message += this.$t('reports.changes.fix', 'en', { version: this.wrapper.fix, url: "/download" });
                        } else {
                            message += this.$t('reports.changes.fix_web', 'en', { version: this.wrapper.fix, url: "/download" });
                        }
                        break;
                    case "wont_fix":
                        message += this.$t('reports.changes.denied', 'en', { state: stateName, reason: this.wrapper.reason });
                        break;
                    default:
                        message += this.$t('reports.changes.state', 'en', { state: stateName });
                        break;
                }
            }

            if (this.wrapper.type !== this.report.type) {
                let typeName = this.$t(`reports.types.content.${this.wrapper.type}.label`);
                message += this.$t('reports.changes.type', { type: typeName });
            }

            if (this.wrapper.product !== this.report.product) {
                let productName = this.$t(`reports.product.content.${this.wrapper.product}`);
                message += this.$t('reports.changes.product', { product: productName });
            }

            let comment = {
                text: message,
                type: "reports",
                source: this.wrapper.id,
                userId: this.user.id,
                displayName: this.user.displayName
            }
            await this.$store.dispatch('reports/updateReport', this.wrapper);
            await this.$store.dispatch('comments/setComment', comment);
        },
        async getReport() {
            if (this.report.id) return;
            await this.$store.dispatch('reports/getReportById', this.$route.query.id);
            this.wrapper = Object.assign({}, this.report);
        },
        async getAuthor() {
            if (this.author.id || this.report.authorId === undefined) return;
            await this.$store.dispatch('auth/getBuilderById', this.report.authorId);
        },
        async remove() {
            await this.$store.dispatch('reports/removeReport', this.wrapper.id);
            this.$router.push(this.localePath('/reports'));
        }
    }
}
</script>