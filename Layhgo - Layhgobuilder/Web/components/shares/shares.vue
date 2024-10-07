<template>
    <v-row>
        <v-col
            cols='12' lg='6' xl='4'
            class='d-flex child-flex'
            v-for='(share,n) in shares'
            :key='n'>
            <l-share :id='share'></l-share>
        </v-col> 
    </v-row>
</template>

<script>
import { access } from '@/config/security'
import Share from './share'

export default {
    mounted() {
        this.$nextTick( () => {
            this.getSharedProjects();
        })
    },
    components: {
        'l-share': Share
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        projects() {
            let projects = this.$store.getters['shared/projects'];
            return (projects) ? projects : {};
        },
        shares() {
            let shares = Object.keys(this.projects)
            .filter(key => { return this.projects[key][this.user.id] === access.view.value || this.projects[key][this.user.id] === access.edit.value });
            return (shares) ? shares : [];
        }
    },
    methods: {
        getSharedProjects() {
            this.$store.dispatch('projects/getSharedByUserId', this.user.id);
        },
    }
}
</script>