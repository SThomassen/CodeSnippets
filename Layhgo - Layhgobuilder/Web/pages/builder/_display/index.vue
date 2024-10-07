<template>
    <v-container>
        <v-row class='mt-2'>
            <v-col>
                <l-header :builder='builder'></l-header>
                <v-row v-if='hasAccess'>
                    <!-- <v-col cols='12' lg='4'>
                        <l-details :builder='builder'></l-details>
                    </v-col> -->

                    <v-col cols='12' lg='8'>
                        <l-projects :projects='projects'></l-projects>
                    </v-col>
                </v-row>
                <v-row no-gutters v-else>
                    <v-col>
                        <span class='text-h2'>{{$t('private_account')}}</span>
                        <p class='title'>{{$t('set_private')}}.</p>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import Header from '@/components/User/Header'
import Projects from '@/components/User/Projects'
import Details from '@/components/User/Details'

export default {
    head() {
        return {
            title: `Layhgobuilder | ${this.builder.displayName}`,
        }
    },
    async mounted() {
        await this.getBuilderByName();
        this.getProjects();
    },
    components: {
        'l-header': Header,
        'l-projects': Projects,
        'l-details': Details
    },
    computed: {
        builder() {
            let builder = this.$store.getters['auth/builder'](this.$route.params.display);
            return (builder) ? builder : {};
        },
        projects() {
            let projects = this.$store.getters['projects/projects'];
            return Object.keys(projects).map(key => projects[key])
            .filter( item => item !== null && item.creatorId === this.builder.id && item.published);
        },
        hasAccess() {
            return (this.builder.incognito !== undefined && !this.builder.incognito) || this.hasPermission(["bypass_incognito"]);
        },
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ ...perms ]);
        },
        async getBuilderByName() {
            return new Promise( async (resolve, reject) => {
                try {
                    if (this.builder.id !== undefined) return resolve();
                    await this.$store.dispatch('auth/getBuilderByName', this.$route.params.display);
                    resolve();
                } catch(err) {
                    reject(err);
                }
            })
        },
        getProjects() {
            this.$store.dispatch('projects/getProjectsByUserId', this.builder.id);
        }
    }
}
</script>