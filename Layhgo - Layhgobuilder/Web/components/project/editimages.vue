<template>
<v-container>
    <l-confirm ref='dialog' @confirm='confirm'></l-confirm>
    <input 
        ref='upload' 
        type="file" 
        accept="image/*" 
        @change='addImage' 
        enctype="multipart/form-data" 
        hidden>

    <v-row no-gutters>
        <v-col>
            <v-card outlined flat>
                <hooper ref='carousel' style='max-height:400px; height:400px!important;' @slide='updateCarousel'>
                    <slide v-for='(image,n) in filterImages' :key='n'>
                        <v-img :src="image.url" contain max-height="400px">
                            <v-row no-gutters align="start" class="fill-height">
                                <v-spacer/>
                                <v-col class="ma-3" cols='auto'>
                                    <v-btn @click.prevent='removeImage(image)'>
                                        <v-icon>mdi-delete</v-icon>
                                    </v-btn>
                                </v-col>
                            </v-row>
                        </v-img>
                    </slide>
                </hooper>
                
                <v-row no-gutters>
                    <v-col cols='12'>
                        <draggable class="grid" style='width:100%;' v-model="filterImages" draggable=".item" @start="drag=true" @end="drag=false;" v-bind="dragOptions">
                            <div style='width:200px!important; cursor: grab; ' class='ma-1 item' v-for='(element,n) in filterImages' :key='n'>
                                <v-card :elevation="(currentSlide === n && element.active) ? 12 : 0">
                                    <v-img 
                                        height='100px'
                                        :src='element.url'
                                        :elevation="(currentSlide === n) ? 5 : 0"
                                        @click="$refs.carousel.slideTo(n)">
                                    </v-img>
                                </v-card>
                            </div>

                            <v-btn slot='header' class='mt-1' style='width:100px; height:100%' @click.prevent='$refs.upload.click()'>
                                <span class='text-h1'>+</span>
                            </v-btn>
                        </draggable>
                    </v-col>
                </v-row>
            </v-card>

            <v-list>
                <v-list-item>
                    <ul>
                        <li>{{$t('first_image_active')}}.</li>
                        <li>{{$t('reorder_images')}}.</li>
                        <li>{{$t('max_images', { number: maxImages})}}</li>
                    </ul>
                </v-list-item>
            </v-list>
        </v-col>
    </v-row>
</v-container>
</template>

<script>
import {
  Hooper,
  Slide
} from 'hooper';

import draggable from 'vuedraggable'
import Confirm from '@/shared/Dialog/Confirm'

export default {
    data() {
        return {
            images: [],
            maxImages: 10,
            currentSlide: 0,
        }
    },
    mounted() {
        let proj = Object.assign({}, this.project);
        this.setImages(proj.images);
    },
    components: {
        Hooper,
        Slide,
        draggable,
        'l-confirm': Confirm
    },
    props: {
        project: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        }
    },
    computed: {
        dragOptions() {
            return {
                animation: 200,
                group: "description",
                disabled: false,
                ghostClass: "ghost"
            };
        },
        filterImages() {
            if (this.project.images === undefined) return [];
            let active = this.project.images.some(el => el.active);
            return this.images.filter(el => el.active || (!active && el.thumbnail));
        }
    },
    methods: {
        confirm(func) {
            func();
        },
        updateCarousel(payload) {
            this.currentSlide = payload.currentSlide;
        },
        addImage(event) {
            if (this.images.length >= this.maxImages && this.images.every(img => img.active)) {
                return;
            }
            
            const files = event.target.files;
            const reader = new FileReader();
            Array.from(files).forEach(file => {
                reader.onload = (e) => {
                    console.log(file);
                    let img = this.images.find(el => !el.active);
                    if (img) {
                        img.active = true;
                        img.updated = true;
                        img.url = e.target.result;
                        img.file = file;
                    } else {
                        this.images.push({
                            index: this.images.length,
                            active: true,
                            updated: true,
                            url: e.target.result,
                            file: file
                        });
                    }
                };
                reader.readAsDataURL(file); 
            });
        },
        setImages(images) {
            this.images = images ? images : [];
            this.images.forEach(img => {
                img.updated = false;
                if (!img.index) {
                    img.index = this.images.indexOf(img);
                }
            })
        },
        removeImage(img) {
            this.$refs.dialog.confirm({ 
                title: this.$t("messages.remove_image.title"), 
                message: this.$t("messages.remove_image.message"), 
                callback: () => {
                    img.active = false;
                    img.updated = true;
                }
            })
        },
        reset() {
            let proj = Object.assign({}, this.project);
            this.setImages(proj.images);
        },
        save() {
            return new Promise(async (resolve, reject) => {
                try {
                    let content = this.images.filter(img => img.updated && img.file).map(img => ({  
                        dest: `projects/${this.project.id}`,
                        name: img.index.toString(),
                        data: img.file,
                        index: img.index,
                        active: img.active
                    }));

                    let uploaded = await this.$store.dispatch('storage/uploadFiles', { files: content })

                    let images = this.project.images.map(img => ({
                        src: `projects/${this.project.id}`,
                        index: img.index,
                        url: img.url,
                        active: img.active,
                        thumbnail: img.thumbnail
                    }))

                    images.forEach(img => {
                        let element = uploaded.find(el => el.index === img.index);
                        if (element) { img.url = element.url; }
                    })

                    this.project.images = images;
                    resolve();
                } catch(err) {
                    reject(err);
                }
            })
        },
    }
}
</script>

<style scoped>
.grid {
    display: flex;
    flex-direction: row;
    overflow-x:auto;
    max-width: 100%;
}
</style>