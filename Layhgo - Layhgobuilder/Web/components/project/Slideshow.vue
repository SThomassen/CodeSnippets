<template>
    <div>
        <v-dialog v-model="dialog" width='80%'>
            <v-card>
                <v-btn icon @click='dialog = false'>
                        <v-icon>mdi-close</v-icon>
                    </v-btn>
                <v-img :src='currentImage' height="100%"></v-img>
            </v-card>
        </v-dialog>

        <v-card outlined>
            <v-container>
                <hooper v-if='!small' group="group" ref='carousel' style='max-height:480px; height:480px!important;'>
                    
                    <slide v-for='(image,n) in images' :key='n'>
                        <v-img :src="image.url" @error="failed(image)">
                            <v-row align="start">
                                <v-col class="ml-3 mt-2" cols='12'>

                                    <v-tooltip right v-if='fullscreen'>
                                        <template v-slot:activator='{ on }'>
                                            <v-btn icon dark @click="openImage(image)" v-on='on'>
                                                <v-icon>mdi-arrow-expand</v-icon>
                                            </v-btn>
                                        </template>
                                        {{ $t('fullscreen' )}}
                                    </v-tooltip>
                                </v-col>    
                            </v-row>
                        </v-img>
                    </slide>

                    <hooper-navigation slot="hooper-addons"></hooper-navigation>
                    <hooper-pagination slot="hooper-addons"></hooper-pagination>
                </hooper>
            </v-container>
        <!-- </v-card>

        <v-card> -->
            <!-- <hooper group="group" v-if='previews' :itemsToShow="6" :centerMode="!small" style="height:100px;">
                <slide v-for='(image,n) in images' :key='n'>
                    <l-image v-if='small' :image='image' @select='openImage(image)'></l-image>
                    <l-image v-else :image='image' @select='$refs.carousel.slideTo(n)'></l-image>
                </slide>

                <hooper-navigation slot="hooper-addons"></hooper-navigation>
                <hooper-pagination slot="hooper-addons"></hooper-pagination>
            </hooper> -->
        </v-card>
    </div>
</template>

<script>
import {
  Hooper,
  Slide,
  Navigation as HooperNavigation,
  Pagination as HooperPagination
  } from 'hooper';
import 'hooper/dist/hooper.css';
import Image from '@/shared/Cards/Image'

export default {
    components: {
        Hooper,
        Slide,
        HooperNavigation,
        HooperPagination,
        'l-image': Image
    },
    props: {
        project: {
            type: Object,
            required: false,
            default: function() {
                return {};
            }
        },
        images: {
            type: Array,
            required: false,
            default: function() {
                return [];
            }
        },
        previews: {
            type: Boolean,
            required: false,
            default: true
        },
        fullscreen: {
            type: Boolean,
            required: false,
            default: true
        },
        small: {
            type: Boolean,
            required: false,
            default: false
        }
    },
    data() {
        return {
            amount: 12,
            dialog: false,
            currentImage: ""
        }
    },
    mounted() {
        this.$nextTick( () => {
            this.getImages();
        });
    },
    methods: {
        openImage(img) {
            this.currentImage = img.url;
            this.dialog = true;
        },
        failed(img) {
            console.log('load img failed', img);
            this.getThumbnail(img);
        },
        // loadStart(img) {
        //     console.log('loading img',img);
        //     if (img.url === undefined) {
        //         getImage(img);
        //     }
        // },
        getImages() {
            this.images.forEach(img => {
                this.getImage(img);
            })
        },
        async getThumbnail(img) {
            let src = `projects/${this.project.id}/thumbnail.jpg`;
            let url = await this.$store.dispatch('projects/getImage', src);
            this.$set(img, 'url', url);
        },
        async getImage(image) {
            if (image.url === undefined && image.src === undefined) {
                let url = await this.$store.dispatch('projects/getImage', `projects/${this.projectId}/thumbnail.jpg`);
                this.$set(image, 'url', url);
            }
            else if (image.url === undefined) {
                let url = await this.$store.dispatch('projects/getImage', image.src);
                this.$set(image, 'url', url);
            }
        },
    }
}
</script>
