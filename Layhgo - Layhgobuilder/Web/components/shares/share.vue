<template>
    <l-project :project='project'></l-project>
</template>

<script>
import Project from '@/components/projects/project'
export default {
    mounted() {
        this.getProject();
    },
    props: {
        id: {
            type: String,
            required: true,
            default: ""
        }
    },
    components: {
        'l-project': Project
    },
    computed: {
        project() {
            console.log(this.id);
            let project = this.$store.getters['projects/projects'][this.id];
            return (project) ? project : {};
        }
    },
    methods: {
        getProject() {
            if (this.project.id !== undefined) return;
            this.$store.dispatch('projects/getProjectById', this.id);
        }
    }
}
</script>