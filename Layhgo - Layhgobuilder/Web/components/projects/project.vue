<template>
 <v-hover v-slot:default="{ hover }">
    <v-card
        tile
        outlined
        dark
        :elevation="hover ? 5 : 0"
        :class="{ 'on-hover': hover }"
        :to='localePath({ name: route, params: { projectName: project.projectName.replace(/[^a-zA-Z0-9]/g,"_") }, query: { id: project.id } })'>
        <v-img 
            :src='image.url'
            @error='failed'
            aspect-ratio="1.5">
            <v-row align="end" class="fill-height">
                <v-col class="py-0">
                    <v-sheet color="rgba(0, 0, 0, .4)">
                        <v-list-item dark>
                            <v-list-item-content>
                                <v-list-item-title class="title">
                                    {{project.projectName}}
                                </v-list-item-title>
                                <v-list-item-subtitle>
                                    <v-row no-gutters>
                                        <v-col>
                                            <v-tooltip right>
                                                <template v-slot:activator="{ on }">
                                                    <v-icon small left v-on="on">mdi-heart</v-icon>
                                                    <span>{{ (stats.votes > 0) ? stats.votes : 0 }}</span>
                                                </template>
                                                <span>{{$t('votes')}}</span>
                                            </v-tooltip>
                                        </v-col>
                                        <v-col>
                                            <v-tooltip right>
                                                <template v-slot:activator="{ on }">
                                                    <v-icon small left v-on="on">mdi-account-group</v-icon>
                                                    <span>{{ (stats.views > 0) ? stats.views : 0 }}</span>
                                                </template>
                                                <span>{{$t('views')}}</span>
                                            </v-tooltip>
                                        </v-col>
                                        <v-col>
                                            <v-tooltip right>
                                                <template v-slot:activator="{ on }">
                                                    <v-icon small left v-on="on">mdi-comment</v-icon>
                                                    <span>{{ (stats.comments > 0) ? stats.comments : 0 }}</span>
                                                </template>
                                                <span>{{$t('comments')}}</span>
                                            </v-tooltip>
                                        </v-col>
                                    </v-row>
                                </v-list-item-subtitle>
                            </v-list-item-content>
                        </v-list-item>
                    </v-sheet>
                </v-col>
            </v-row>
        </v-img>
    </v-card>
 </v-hover>
</template>

<script>
import Image from '@/shared/Cards/Image'

export default {
    mounted() {
        this.getThumbnail();
    },
    components: {
        'l-img': Image
    },
    props: {
        project: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        },
        route: {
            type: String,
            required: false,
            default: "projects-projectName"
        }
    },
    computed: {
        stats() {
            return (this.project['--stats--']) ? this.project['--stats--'] : {};
        },
        activeThumbnail() {
            let index = 0;
            for (let i in this.project.images) {
                if (this.project.images[i].active) { 
                    index = i; 
                    break;
                }
            }
            return index;
        },
        image() {
            if (this.project.images === undefined) return {};
            let image = this.project.images[this.activeThumbnail];
            return (image.url) ? image : {};
        }
    },
    methods: {
        async getThumbnail() {
            if (this.project.images === undefined)
            {
                let src = `projects/${this.project.id}/thumbnail.jpg`;
                let url = await this.$store.dispatch('projects/getImage', src);
                this.project.images = [ { src: src, url: url, active: true, thumbnail: true } ];
                this.$set(this.project.images[this.activeThumbnail], 'url', url);
                this.$store.commit('projects/setProject', { id: this.project.id, images: this.project.images });
                return;
            }
            
            let img = this.project.images[this.activeThumbnail];
            if (img.src === undefined || img.url !== undefined) return;

            let src = (img.src) ? img.src : img.path;
            let url = await this.$store.dispatch('projects/getImage', src);
            this.$set(this.project.images[this.activeThumbnail], 'url', url);
            this.$store.commit('projects/setProject', { id: this.project.id, images: this.project.images });
        },
        failed(obj, str) {
            console.log(obj,str);
        }
    }
}
</script>

<style scoped>
.v-card {
  transition: opacity .4s ease-in-out;
}

.v-card:not(.on-hover) {
  opacity: 0.6;
 }
 </style>