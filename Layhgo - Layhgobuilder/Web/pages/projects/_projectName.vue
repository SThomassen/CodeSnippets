<template>
    <v-container>
        <l-report ref='report' @submit='reportProject'></l-report>
        <v-row class='mt-2'>
            <v-col cols='12' xl='8' offset-xl='2'>
                <!-- <l-slideshow :images='slideshow'/> -->

                <v-toolbar elevation="0" outlined>
                    <v-tooltip bottom>
                        <template v-slot:activator='{ on }'>
                            <v-btn icon v-on='on' @click.prevent='back()'>
                                <v-icon>mdi-arrow-left</v-icon>
                            </v-btn>
                        </template>
                        {{$t('back')}}
                    </v-tooltip>

                    <span class='text-h4'>{{project.projectName}}</span>
                    <v-spacer></v-spacer>                    

                    <l-edit v-if='auth' :project='project'></l-edit>
                </v-toolbar>

                
                <v-row class='mt-4'>
                    <v-col cols='12'>
                        <l-slideshow :project='project' :images='slideshow()'/>
                    </v-col>
                    <v-col cols='12'>
                        <v-card tile>
                            <v-list two-line> 
                                <v-list-item :to='localePath(to)' :inactive='project.showcase'>
                                    <v-list-item-avatar>
                                        <l-avatar :builder='avatar'></l-avatar>
                                    </v-list-item-avatar>
                                    <v-list-item-content>
                                        <v-list-item-title>
                                            {{ $t('created_by')}}: {{creatorName}}
                                            <!-- Created By: {{creator.displayName}} -->
                                        </v-list-item-title >
                                        <v-list-item-subtitle>
                                            {{ $t('created_at') }}: 
                                            <l-timestamp :date='project.creationTime' format="short"></l-timestamp><!--dddd, MMMM Do YYYY-->
                                        </v-list-item-subtitle>
                                    </v-list-item-content>

                                    <v-list-item-action v-if='$vuetify.breakpoint.smAndUp'>
                                        <v-row align="center" no-gutters>
                                            <v-col>
                                                <v-tooltip right>
                                                    <template v-slot:activator="{ on }">
                                                        <v-icon left v-on="on">mdi-account-group</v-icon>
                                                    </template>
                                                    <span>{{ $t('views') }}</span>
                                                </v-tooltip>
                                            </v-col>
                                            <v-col class='ml-2 mr-8'>
                                                <span>{{ (stats.views > 0) ? stats.views : 0}}</span>
                                            </v-col>
                                            <v-col>
                                                <v-tooltip right>
                                                    <template v-slot:activator="{ on }">
                                                        <v-btn icon @click.prevent='setVote' v-on="on">
                                                            <v-icon v-if='vote || auth'>mdi-heart</v-icon>
                                                            <v-icon v-else>mdi-heart-outline</v-icon>
                                                        </v-btn>
                                                    </template>
                                                    <span>{{ $t('votes') }}</span>
                                                </v-tooltip>
                                            </v-col>
                                            <v-col class='ml-2 mr-8'>
                                                <span>{{ (stats.votes > 0) ? stats.votes : 0}}</span>
                                            </v-col>
                                            <v-col class='ml-2'>
                                                <l-share ref='share' :project='project'></l-share>
                                            </v-col>

                                            <v-tooltip bottom  v-if='project.allowAccess || project.showcase || auth'>
                                                <template v-slot:activator='{ on }'>
                                                    <v-btn icon @click.prevent='openProject' v-on='on'>
                                                        <v-icon>mdi-folder-open</v-icon>
                                                    </v-btn>
                                                </template>
                                                {{ $t('open_project', { name: project.projectName }) }}
                                            </v-tooltip>

                                            <v-tooltip bottom>
                                                <template v-slot:activator='{ on }'>
                                                    <v-btn icon @click.prevent='$refs.report.confirm(project.projectName)' v-on='on'>
                                                        <v-icon>mdi-flag</v-icon>
                                                    </v-btn>
                                                </template>
                                                {{ $t('report_project', { name: project.projectName }) }}
                                            </v-tooltip>
                                        </v-row>
                                    </v-list-item-action>

                                </v-list-item>
                            </v-list>
                        </v-card>

                        <v-card v-if='$vuetify.breakpoint.xsOnly'>
                            <v-row class='pa-3' align="center" no-gutters>
                                <v-col cols='auto'>
                                    <v-tooltip right>
                                        <template v-slot:activator="{ on }">
                                            <v-icon left v-on="on">mdi-account-group</v-icon>
                                        </template>
                                        <span>{{ $t('views') }}</span>
                                    </v-tooltip>
                                </v-col>
                                <v-col cols='auto' class='ml-2 mr-8'>
                                    <span>{{ (stats.views > 0) ? stats.views : 0}}</span>
                                </v-col>
                                <v-col cols='auto'>
                                    <v-tooltip right>
                                        <template v-slot:activator="{ on }">
                                            <v-btn icon @click.prevent='setVote' v-on="on">
                                                <v-icon v-if='vote || auth'>mdi-heart</v-icon>
                                                <v-icon v-else>mdi-heart-outline</v-icon>
                                            </v-btn>
                                        </template>
                                        <span>{{ $t('votes') }}</span>
                                    </v-tooltip>
                                </v-col>
                                <v-col cols='auto' class='ml-2 mr-8'>
                                    <span>{{ (stats.votes > 0) ? stats.votes : 0}}</span>
                                </v-col>
                                <v-col cols='auto' class='ml-2 mr-8'>
                                    <l-share ref='share' :project='project'></l-share>
                                </v-col>

                                <v-col cols='auto' v-if='project.allowAccess'>
                                    <v-tooltip bottom>
                                        <template v-slot:activator='{ on }'>
                                            <v-btn icon @click='openProject' v-on='on'>
                                                <v-icon>mdi-folder-open</v-icon>
                                            </v-btn>
                                        </template>
                                        {{ $t('open_project', { name: project.projectName }) }}
                                    </v-tooltip>
                                </v-col>
                            </v-row>
                        </v-card>

                        <v-card tile class='pa-3'>
                            <div class='mb-2 text-h6'>{{$t('description')}}</div>
                            <span v-html="project.description"></span>
                        </v-card>

                        <v-card tile class='pa-3'>
                            <l-comments v-if='project.allowComments' type='projects' :target='project' :requireUser='false' :showComments='true'></l-comments>
                        </v-card>

                    </v-col>
                </v-row>
            </v-col>

            <template v-if='hasPermission([]) && (project.state === "pending" || project.state === undefined)'>
                <v-col cols='12' xl='2'>
                    <v-toolbar elevation="0" outlined>
                        {{$t('moderate')}}
                    </v-toolbar>
                    <v-card>
                        <v-card-text>
                            <v-row>
                                <v-col>
                                    <span>{{$t('last_update')}}:</span>
                                    <l-timestamp v-if='project.lastUpdate' :date='project.lastUpdate' format="short"></l-timestamp><!-- dddd, MMMM Do YYYY -->
                                </v-col>
                            </v-row>
                        
                            <v-row no-gutters>
                                <v-btn-toggle borderless v-model='ticket.state'>
                                    <v-col cols='6'>
                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn block v-on='on'>
                                                    <v-icon>mdi-check</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t('approve')}}
                                        </v-tooltip>
                                    </v-col>
                                    <v-col cols='6'>
                                        <v-tooltip bottom>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn block v-on='on'>
                                                    <v-icon>mdi-cancel</v-icon>
                                                </v-btn>
                                            </template>
                                            {{$t('disapprove')}}
                                        </v-tooltip>
                                    </v-col>
                                </v-btn-toggle>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-textarea v-model='ticket.reason' outlined :label='$t("reason")'>

                                    </v-textarea>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-btn block @click.prevent='submitTicket'>
                                        {{$t('submit')}}
                                    </v-btn>
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-card>
                </v-col>
            </template>
            <template v-else-if='hasPermission([])'>
                <v-col cols='12' xl='2'>
                    <v-toolbar elevation="0" outlined>
                        {{$t('moderate')}}
                    </v-toolbar>
                    <v-card>
                        <v-card-text>
                            <v-row>
                                <v-col cols="12">
                                    <span>{{$t('last_update')}}:</span>
                                    <l-timestamp v-if='project.lastUpdate' :date='project.lastUpdate' format="short"></l-timestamp><!-- dddd, MMMM Do YYYY -->
                                </v-col>

                                <v-col cols="12">
                                    <span>{{$t('moderator_time')}}: <l-timestamp :date='project.moderatorTime' format="short"></l-timestamp></span>
                                </v-col>

                                <v-col cols="12">
                                    <span>{{$t('moderator')}}: {{project.moderatorName}}</span>
                                </v-col>

                                <v-col cols="12">
                                    <span>{{$t('state')}}: {{ $t(project.state) }}</span>
                                </v-col>

                                <v-col cols="12">
                                    <v-textarea :value='project.reason' readonly outlined :label='$t("reason")'>
                                    </v-textarea>
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-card>
                </v-col>
            </template>
        </v-row>
    </v-container>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Comments from '@/components/comments/comments'
import Slideshow from '@/components/project/Slideshow'
import Timestamp from '@/shared/Input/Timestamp'
import Share from '@/shared/Media/Share'
import Report from '@/shared/Dialog/Report'

import Edit from '@/components/project/edit'

import debug from '@/config/debug'
import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            vote: false,
            ticket: {},
            showcaseAvatar: {
                photoURL: "lb_logo.png"
            },
            states: [ "approved", "disapproved" ]
        }
    },
    head () {
        return {
            title: `Layhgobuilder | ${this.project.projectName}`,
            link: [{ rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }],
            meta: [
                { name:"description",content: "Current page description." },
                // hid is used as unique identifier. Do not use `vmid` for it as it will not work
                { property:"og:title", content:"European Travel Destinations" },
                { property:"og:description", content:"Offering tour packages for individuals or groups." },
                { property:"og:og:image", content:"https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-1.2.1&w=1000&q=80" },
                { property:"og:og:image:secure_url", content:"https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-1.2.1&w=1000&q=80" },
                { property:"og:url", content:"http://euro-travel-example.com/index.htm" },

                { hid: 'description', name: 'description', content: 'My custom description' },
                { hid: 'og:image', name: 'og:image', content: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-1.2.1&w=1000&q=80' },
                { hid: 'og:image:secure_url', name: 'og:image:secure_url', content: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-1.2.1&w=1000&q=80' },

                { name: 'twitter:title', content: this.project.projectName },
                { name: 'twitter:description', content: this.project.description },
                { name: 'twitter:image', content: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-1.2.1&w=1000&q=80' },
                { name: 'twitter:image:secure_url', content: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-1.2.1&w=1000&q=80' },
                { name: 'twitter:card', content: 'summary_large_image' }
            ]
        }
    },
    async mounted() {
        await this.getProject();
        this.getCreator();
        // this.getSharedProjects();

        this.getVote();
        this.setView();
    },
    components: {
        'l-comments': Comments,
        'l-timestamp': Timestamp,
        'l-slideshow': Slideshow,
        'l-share': Share,
        'l-edit': Edit,
        'l-avatar': Avatar,
        'l-report': Report
    },
    computed: {
        lorum() {
            return debug.lorum;
        },
        avatar() {
            let avatar = this.project.showcase ? this.showcaseAvatar : this.creator;
            return avatar;
        },
        creatorName() {
            let name = this.project.showcase ? "Layhgobuilder Team" : this.creator.displayName;
            return name;
        },
        project() {
            let project = this.$store.getters['projects/projects'][this.$route.query.id];
            return (project) ? project : {};
        },
        stats() {
            return (this.project['--stats--']) ? this.project['--stats--'] : {};
        },
        creator() {
            let builders = this.$store.getters['auth/builders'];
            let creator = Object.values(builders).filter(item => item.id === this.project.creatorId)[0];
            return (creator) ? creator : {};
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        auth() {
            return this.project.creatorId === this.user.id;
        },
        to() {
            let username = (this.creator.username === undefined) ? "notfound" : this.creator.username;
            return { name: "builder-display", params: { display: username } };
        },
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission'](perms);
        },
        slideshow() {
            if (this.project.images === undefined) return [];

            let active = this.project.images.some(el => el.active);
            return Object.values(this.project.images).filter(img => { return img.active || (!active && img.thumbnail); });
        },
        back() {
            this.$router.go(-1);
        },
        async getProject() {
            if (this.project.id !== undefined) return;

            await this.$store.dispatch('projects/getProjectById', this.$route.query.id);

            if (this.project.images === undefined)
            {
                let src = `projects/${this.project.id}/thumbnail.jpg`;
                this.project.images = [ { src: src, active: true, thumbnail: true } ];
            }

            let thumbnails = this.project.images.filter(el => el.thumbnail);
            thumbnails.forEach(async thumbnail => {

                let src = thumbnail.src.endsWith("thumbnail.jpg") ? thumbnail.src : `${thumbnail.src}/thumbnail.jpg`;
                let url = await this.$store.dispatch('projects/getImage', src);
                
                thumbnail.url = url;
                this.$set(thumbnail, 'url', url);
            });
            this.$store.commit('projects/setProject', { id: this.project.id, images: this.project.images });
        },
        // getSharedProjects() {
        //     this.$store.dispatch('shared/getSharedByProjectId', this.project.id);
        // },
        getCreator() {
            if (this.creator.id !== undefined) return;

            return new Promise( async (resolve, reject) => {
                try {
                    await this.$store.dispatch('auth/getBuilderById', this.project.creatorId);
                    resolve();
                } catch (err) {
                    reject(err);
                }
            })
        },
        getVote() {
            this.vote = localStorage[`project_${this.$route.query.id}_vote`];
        },
        async setVote() {
            if (this.auth) return;
            
            localStorage.setItem(`project_${this.$route.query.id}_vote`, !this.vote);
            try {
                await this.$store.dispatch('projects/setVote', { 
                    projectId: this.$route.query.id,
                    creatorId: this.project.creatorId,
                    vote: !this.vote,
                    votes: this.stats.votes
                });
                this.vote = !this.vote;
            } catch(error) {
                console.log(error);
            }
        },
        async setView() {
            let viewed = localStorage[`project_${this.$route.query.id}_view`];
            if (viewed || this.auth) return;

            localStorage.setItem(`project_${this.$route.query.id}_view`, true);
            try {
                await this.$store.dispatch('projects/setView', {
                    projectId: this.$route.query.id,
                    views: this.stats.views
                });
            } catch(err) {
                console.log(err);
            }
        },
        openProject() {
            this.$bus.$emit('onOpenProject', this.$route.query.id);
        },
        async submitTicket() {
            await this.$store.dispatch('projects/updateProject', {
               id: this.project.id,
               skipUpdate: true,
               state: this.states[this.ticket.state],
               reason: this.ticket.reason,
               moderatorId: this.user.id,
               moderatorName: this.user.username,
               moderatorTime: new firebase.firestore.FieldValue.serverTimestamp()
            });

            
            let message = {
                reason: ticket.reason,
                description: ticket.description,
                userId: this.project.creatorId,
                userName: this.project.creatorName,
                moderatorId: this.user.id,
                moderatorName: this.user.username,
                moderatorTime: new firebase.firestore.FieldValue.serverTimestamp(),
            }
            await this.$store.dispatch('auth/warnUser', message);


            let project = await this.$store.dispatch('projects/getNextModerateProject');
            if (project) {
                let path = this.localePath({ name: 'projects-projectName', params: { projectName: project.projectName.replace(/[^a-zA-Z0-9]/g,"_") }, query: { id: project.id } })
                this.$router.push(path);
            } else {
                this.$bus.$emit('onShowMessage', {
                    message: this.$t('no_moderation')
                })
            }
        },
        async reportProject(ticket) {
            await this.$store.dispatch('projects/reportProject', {
                projectName: this.project.projectName,
                projectId: this.project.id,
                creatorName: this.project.creatorName,
                creatorId: this.project.creatorId,
                creationTime: this.project.creationTime,
                invoked: this.project.invoked,
                ...ticket
            });
        }
    }
}
</script>