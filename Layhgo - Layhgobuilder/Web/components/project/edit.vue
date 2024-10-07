<template>
<div style='overflow:hidden'>
    <v-tooltip bottom>
        <template v-slot:activator='{ on }'>
            <v-btn icon @click='open' v-on='on'>
                <v-icon>mdi-pencil</v-icon>
            </v-btn>
        </template>
        <span>{{$t('edit_project')}}</span>
    </v-tooltip>

    <v-dialog 
        v-model='dialog'
        max-width='800px'
        transition="dialog-bottom-transition"
        @keydown.esc="cancel"
        @click:outside='cancel' >

        <v-card>
            <v-app-bar flat>
                <v-btn icon @click='cancel'>
                    <v-icon>mdi-close</v-icon>
                </v-btn>

                <v-toolbar-title>{{$t('project_settings')}}</v-toolbar-title>
                <v-spacer></v-spacer>

                <v-btn :loading='loading' outlined color='info' @click='save'>{{$t('save')}}</v-btn>
            </v-app-bar>
            
            <v-tabs fixed-tabs v-model="tab">
                <v-tab>{{$t('project')}}</v-tab>
                <v-tab>{{$t('images')}}</v-tab>
                <v-tab v-if='hasPermission(["share_project"])'>{{$t('share')}}</v-tab>
            </v-tabs>        

            <v-tabs-items v-model="tab">
                <v-tab-item>
                    <l-project :project='wrapper'></l-project>
                </v-tab-item>
                <v-tab-item>
                    <l-images ref='images' :project='wrapper'></l-images>
                </v-tab-item>
                <v-tab-item>
                    <l-share ref='share' :project='wrapper' :id='wrapper.id'></l-share>
                </v-tab-item>
            </v-tabs-items>
            
        </v-card>
    </v-dialog>
</div>
</template>

<script>
import EditProject from './editproject'
import EditShare from './editshare'
import EditImages from './editimages'

export default {
    data() {
        return {
            tab: 0,
            dialog: false,
            loading: false,
            wrapper: {}
        }
    },
    components: {
        'l-project': EditProject,
        'l-share': EditShare,
        'l-images': EditImages
    },
    computed: {
        project() {
            let project = this.$store.getters['projects/projects'][this.$route.query.id];
            return (project) ? project : {};
        },
    },
    methods: {
        changes() {
            this.settingChanged = JSON.stringify(this.project) !== JSON.stringify(this.wrapper);
            return this.settingChanged;
        },
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission'](perms);
        },
        cancel() {
            this.wrapper = JSON.parse(JSON.stringify(this.project));

            if (this.$refs.share !== undefined) this.$refs.share.reset();
            if (this.$refs.images !== undefined) this.$refs.images.reset();

            this.dialog = false;
        },
        open() {
            this.dialog=true;
            this.wrapper = JSON.parse(JSON.stringify(this.project)); // Need to parse for deep clone
        },
        async save() {
            try {
                this.loading = true;
                if (this.$refs.share) {
                    await this.$refs.share.save();
                }

                if (this.$refs.images) {
                    await this.$refs.images.save();
                }

                if (this.wrapper.shared) {
                    Object.keys(this.wrapper.shared).forEach(async key => {
                        let newShare = this.wrapper.shared[key];
                        let oldShare = this.project.shared ? this.project.shared[key] : "none";
                        if (newShare !== oldShare) {
                            let value = 0;
                            if (oldShare === "none" && newShare !== "none") {
                                value = 1; //increment
                            } else if (oldShare !== "none" && newShare === "none") {
                                value = -1;//decrement
                            }
                            console.log(key, value);
                            await this.$store.dispatch('auth/setSharedByUserId', { id: key, value: value});
                        }
                    })
                }

                await this.$store.dispatch('projects/updateProject', this.wrapper);

                let stats = {};
                if (this.project.published !== this.wrapper.published) {
                    let value = (this.wrapper.published) ? 1 : -1;
                    stats = { ...stats, published: value };

                    await this.$store.dispatch('auth/updateStats', { id: this.project.creatorId, stats: { 'published': value }} );
                    this.project.published = this.wrapper.published;
                }
                if (this.project.showcase !== this.wrapper.showcase) {
                    let value = this.wrapper.showcase ? 1 : -1;
                    stats = { ...stats, showcase: value };

                    this.project.showcase = this.wrapper.showcase;
                }

                await this.$store.dispatch('projects/updateStats', stats);
                this.$bus.$emit('onShowMessage', {
                    message: this.$t('project_saved', { name: this.project.projectName})
                })
                this.loading = false;
                this.dialog = false;
            } catch(err) {
                console.log(err);
                this.$bus.$emit('onShowMessage', {
                    message: err,
                    color: "error"
                })
            }
        },
    }
}
</script>

<style>
    .ql-toolbar {
        background-color: white;
    }
</style>