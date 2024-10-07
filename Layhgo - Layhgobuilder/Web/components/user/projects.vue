<template>
    <v-container fluid>
        <v-row>
            <v-col cols='12'>
                <h3>{{$t('projects')}}</h3>
            </v-col>
            <template v-if='!empty'>
                <v-col
                    v-for="(project,n) in projects"
                    :key="n"
                    cols="4">
                    <l-project :project='project'></l-project>
                </v-col>
            </template>

            <template v-else>
                <v-col>
                    <span class='subtitle-2'>{{$t('no_projects')}}.</span>
                </v-col>
            </template>
        </v-row>
    </v-container>
</template>

<script>
import Project from '@/components/projects/project'

export default {
    props: {
        projects: {
            type: Array,
            required: true,
            default: function() {
                return [];
            }
        }
    },
    components: {
        'l-project': Project
    },
    computed: {
        empty() {
            return this.projects.length <= 0;
        }
    },
}
</script>