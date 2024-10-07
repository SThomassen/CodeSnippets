<template>
    <v-card class="pa-6">
        <h1>{{project.title}}</h1>

        <v-tooltip left>
            <template v-slot:activator='{ on }'>
                <v-list-item v-on='on'>
                    <v-list-item-icon>
                        <v-icon>mdi-account</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        {{project.creator}}
                    </v-list-item-content>
                </v-list-item>
            </template>
            {{$t('creator')}}
        </v-tooltip>

        <v-tooltip left>
            <template v-slot:activator='{ on }'>
                <v-list-item v-on='on'>
                    <v-list-item-icon>
                        <v-icon>mdi-factory</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <span v-html="project.company"></span>
                    </v-list-item-content>
                </v-list-item>
            </template>
            {{$t('company')}}
        </v-tooltip>
        
        <v-tooltip left>
            <template v-slot:activator='{ on }'>
                <v-list-item v-on='on'>
                    <v-list-item-icon>
                        <v-icon>mdi-calendar</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        {{project.date}}
                    </v-list-item-content>
                </v-list-item>
            </template>
            {{$t('date')}}
        </v-tooltip>
        
        <v-row no-gutters>
            <v-col cols="12">
                <span class="caption font-weight-bold">{{$t("description")}}</span>
            </v-col>
            <v-col cols="12">
                <s-html :text="description"></s-html>
                <!-- <span class="caption">{{description}}</span> -->
            </v-col>

            <template v-if="project.tech.length > 0">
                <v-col cols="12" class="mt-4">
                    <span class="caption font-weight-bold">{{$t("technology")}}</span>
                </v-col>
                <v-col cols="12">
                    <v-chip-group column>
                        <v-chip label v-for="item in project.tech" :key="item">{{item}}</v-chip>
                    </v-chip-group>
                </v-col>
            </template>
            
            <template v-if="project.social.length > 0">
                <v-col cols="12" class="mt-4">
                    <span class="caption font-weight-bold">{{$t("platform")}}</span>
                </v-col>
                <v-col cols="12">
                    <v-tooltip top v-for="item in project.social" :key="item.url">
                        <template v-slot:activator='{ on }'>
                            <v-btn icon v-on='on' @click="open(item.url)">
                                <v-icon>{{item.icon}}</v-icon>
                            </v-btn>
                        </template>
                        <span v-html="item.text"></span>
                    </v-tooltip>
                </v-col>
            </template>
        </v-row>
    </v-card>
</template>

<script>
    import HTML from '@/shared/Html'
export default {
    props: {
        project: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        }
    },
    components: {
        's-html': HTML
    },
    computed: {
        description() {
            return this.project.description[this.lang];
        },
        lang() {
            return this.$i18n.locale;
        }
    },
    methods: {
        open(url) {
            window.open(url, '_blank');
        }
    }
}
</script>