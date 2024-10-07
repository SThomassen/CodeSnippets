<template>
<v-card
    height="240"
    tile
    dark>
    <l-images 
        destination='coverURL'
        title='Update Cover' 
        source='cover' 
        ref='cover' 
        :url='cover'>
    </l-images>

    <v-hover>
        <v-img
            height="100%"
            gradient="to bottom left, rgba(255,255,255,.33), rgba(255,255,255,.4), rgba(50,50,50,.7)"
            slot-scope='{ hover } '
            :src="cover">
            <v-row align="end" class="fill-height">
                <v-col align-self="start" class="mt-3 ml-3" cols='12'>
                    <l-avatar :builder='builder' :auth='auth'></l-avatar>

                    <v-btn 
                        top
                        right
                        icon
                        absolute
                        v-if='hover && auth'
                        @click='$refs.cover.open'>
                        <v-icon>mdi-pencil</v-icon>
                    </v-btn>          
                </v-col>
                
                <v-col class="py-0">
                    <v-list-item color="rgba(1, 0, 0, .4)" dark>
                        <v-list-item-action>
                            <v-tooltip bottom>
                                <template v-slot:activator='{ on }'>
                                    <v-btn icon v-on='on' @click.prevent='back'>
                                        <v-icon>mdi-arrow-left</v-icon>
                                    </v-btn>
                                </template>
                                {{$t('back')}}
                            </v-tooltip>
                        </v-list-item-action>
                        <v-list-item-content>
                            <v-list-item-title class="title">
                                {{builder.displayName}}
                                <!-- <l-presence :server='builder.lastSignInTime'></l-presence> -->
                            </v-list-item-title>
                            <v-list-item-subtitle>
                                {{$t('builder_since')}}: 
                                <l-timestamp :date='builder.creationTime' format="short"></l-timestamp><!--dddd, MMMM Do YYYY-->
                            </v-list-item-subtitle>
                        </v-list-item-content>
                    </v-list-item>
                </v-col>
            </v-row>
        </v-img>
    </v-hover>
</v-card>
</template>

<script>
import Avatar from './Avatar'
import Images from '@/shared/Dialog/Images'
import Timestamp from '@/shared/Input/Timestamp'
import Presence from '@/shared/Notifications/Presence'

export default {
    components: {
        'l-avatar': Avatar,
        'l-images': Images,
        'l-timestamp': Timestamp,
        'l-presence': Presence
    },
    props: {
        builder: {
            type: Object,
            required: true,
            default: function() {
                return {}
            }
        }
    },
    data() {
        return {
            defaultImg: '' // 'https://cdn.pixabay.com/photo/2020/01/11/10/08/landscape-4757115_960_720.jpg'
        }
    },
    computed: {
        cover() {
            return (this.builder.coverURL) ? this.builder.coverURL : this.defaultImg;
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        auth() {
            return this.builder.id === this.user.id;
        },
    },
    methods: {
        back() {
            this.$router.go(-1);
        }
    }
}
</script>