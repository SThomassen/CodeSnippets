<template>
    <v-container>
        <!-- <v-row>
            <v-col align-self="center">
            </v-col>
            <v-col cols='12'></v-col>
            <v-col offset="2"></v-col>
            <v-spacer></v-spacer>
        </v-row> -->
        <v-row class="mt-2">
            <v-col>
                <span class='text-h2'>{{ $t('instructions.title') }}</span>
                <v-divider></v-divider>
                <!-- <v-tabs v-model='tab' grow>
                    <v-tab
                        v-for="item in products"
                        :key="item.value">
                        {{ item.text }}
                    </v-tab>
                </v-tabs> -->
                
                <v-row class='mt-2'>
                    <v-col cols='auto'>
                        <v-card
                            height="70vh"
                            width="256">
                            <v-navigation-drawer permanent>
                                <v-list-item dense>
                                    <v-list-item-content>
                                        <v-list-item-title class="text-h6">
                                            Content
                                        </v-list-item-title>
                                    </v-list-item-content>
                                </v-list-item>

                                <v-divider></v-divider>

                                <v-list dense nav>
                                    <v-list-item-group>
                                        <template v-for="(item, i) in items">
                                            <v-list-group :value='item.header.expand' :key="i" :prepend-icon="item.header.icon"> 
                                                <template v-slot:activator>
                                                    <v-list-item-title>{{ $t(`instructions.${product}.${item.header.title}.title`) }}</v-list-item-title>
                                                </template>
                                                    <v-list-item
                                                        v-for="content in item.content"
                                                        :key="content"
                                                        link
                                                        @click="changeSubject(content)">
                                                        <v-list-item-content>
                                                            <v-list-item-title>{{ $t(`instructions.${product}.${content}.title`) }}</v-list-item-title>
                                                        </v-list-item-content>
                                                    </v-list-item>
                                            </v-list-group>
                                        </template>
                                    </v-list-item-group>
                                </v-list>
                            </v-navigation-drawer>
                        </v-card>
                    </v-col>

                    <v-col>
                        <!-- <div v-html='$t(`${currentSubject}.content`)'></div> -->
                        <l-html :htmlContent='$t(`${currentSubject}.content`)'></l-html>
                    </v-col>
                    <!-- <v-col v-for='(url, i) in urls' :key='i' cols='12' lg='6' xl='4'>
                    <iframe 
                        width="100%" 
                        height="315"
                        class='pa-3' 
                        :src='url' 
                        frameborder="0" 
                        allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" 
                        allowfullscreen>
                    </iframe>
                    </v-col> -->
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import HTML from '@/shared/Input/Html'
import instructions from '@/config/instructions'

export default {
    head() {
        return {
            title: `Layhgobuilder | Instructions`,
        }
    },
    data() {
        return {
            selected: 0,
            tab: 0,
            products: [ { value: 'web', text: "Web" }, { value: 'fieldlab', text: "Fieldlab" }, { value: 'basic', text: "Basic" }, { value: 'pro', text: "Pro" },],
            page: {
                platform: 'web',
                subject: 'getting_started.introduction'
            },
            urls: [
                "https://www.youtube.com/embed/Qcxd0Fvk3OM",
                "https://www.youtube.com/embed/EyhJ4TneE_s",
                "https://www.youtube.com/embed/i8qYVUaP5g0",
                "https://www.youtube.com/embed/nWfiCU_Vdiw",
                "https://www.youtube.com/embed/XD_SB_aygs8",
                "https://www.youtube.com/embed/MZkzJnu_E94"
            ],
            right: null,
        }
    },
    components: {
        'l-html': HTML
    },
    computed: {
        items() {
            switch (this.product) {
                case "web":
                    return instructions.web;
                case "fieldlab":
                    return instructions.fieldlab;
                case "basic":
                    return instructions.basic;
                case "pro":
                    return instructions.pro;
            }
        },
        product() {
            return this.products[this.tab].value;
        },
        currentSubject() {
            return `instructions.${this.product}.${this.page.subject}`;
        }
    },
    methods: {
        changePlatform(platform) {
            this.page.platform = platform
        },
        changeSubject(content) {
            this.page.subject = content;
        }
    }
}
</script>
