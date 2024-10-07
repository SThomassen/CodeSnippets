<template>
    <div>
        <l-images 
            destination='photoURL'
            title='Update Avatar' 
            source='avatar' 
            ref='avatar' 
            :url='avatar'>
        </l-images>
        
        <v-hover>
            <v-avatar
                size="128"
                slot-scope="{ hover }"
                @click="open">
                <v-img :src='avatar' @error="failed">
                    <v-container
                        v-if="hover && auth"
                        transition="fade-transition"
                        class="display-1 white--text"
                        style="height:120%; background-color: rgba(0, 0, 0, 0.3);">
                        <v-icon dark>mdi-pencil</v-icon>
                    </v-container>
                </v-img>
            </v-avatar>
        </v-hover>
    </div>
</template>

<script>
import Images from '@/shared/Dialog/Images'

export default {
    components: {
        'l-images': Images,
    },
    props: {
        builder: {
            type: Object,
            required: true,
            default: function() {
                return {}
            }
        },
        auth: {
            type: Boolean,
            required: false,
            default: false
        }
    },
    data() {
        return {
            dialog: false,
            failLoad: false,
            defaultImg: 'https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png'
        }
    },
    computed: {
        avatar() {
            return (this.builder.photoURL && !this.failLoad) ? this.builder.photoURL : this.defaultImg;
        }
    },
    methods: {
        open() {
            if (this.auth)
                this.$refs.avatar.open();
        },
        failed() {
            this.failLoad = true;
        }
    }
}
</script>