<template>
<span style='width:100%'>
    <l-confirm ref='dialog' @confirm='confirm'></l-confirm>

    <v-hover v-slot:default="{ hover }">
        <v-list-item three-line>
            <v-list-item-avatar size='48'>
                <l-avatar :builder='builder'></l-avatar>
            </v-list-item-avatar>

            <v-list-item-content>
                <v-list-item-title class='' v-text='comment.displayName'></v-list-item-title>

                <template v-if='edit'>
                    <v-textarea 
                        :rows='1'
                        :row-height="24"
                        auto-grow
                        :placeholder="$t('place_comment')"
                        v-model='comment.text'>
                    </v-textarea>
                </template>
                <template v-else>
                    <l-html class="text--secondary" :htmlContent='comment.text'></l-html>
                    <!-- <div class="text--secondary" v-html="comment.text"></div> -->
                </template>

                <v-list-item-subtitle class='caption'>
                    <v-row no-gutters align="center">
                        <v-col>
                            <l-timestamp :date='comment.created' format='long'></l-timestamp><!--DD-MM-YYYY HH:mm:ss-->
                        </v-col>

                        <v-spacer></v-spacer>

                        <template v-if='auth'>
                            <v-col cols='auto' v-show="hover && !edit">
                                <v-btn text x-small @click.prevent='modify'>
                                    <v-icon left>mdi-pencil</v-icon>
                                    {{$t('edit')}}
                                </v-btn>
                                <v-btn text x-small color='error' @click.prevent='$refs.dialog.confirm({ 
                                        title: $t("messages.remove_comment.title"), 
                                        message: `${$t("messages.remove_comment.message")}?`, 
                                        callback: remove
                                    })'>
                                    <v-icon left>mdi-delete</v-icon>
                                    {{$t('delete')}}
                                </v-btn>
                            </v-col>
                            <v-col cols='auto' v-if='edit'>
                                <template>
                                    <v-btn text x-small @click.prevent='cancel'>
                                        {{$t('cancel')}}
                                    </v-btn>
                                    <v-btn text x-small color='info' @click.prevent='save'>
                                        <v-icon left>mdi-save-content</v-icon>
                                        {{$t('save')}}
                                    </v-btn>
                                </template>
                            </v-col>
                        </template>
                    </v-row>
                </v-list-item-subtitle>

                <v-divider></v-divider>
            </v-list-item-content>            
        </v-list-item>
    </v-hover>
</span>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Timestamp from '@/shared/Input/Timestamp'
import Confirm from '@/shared/Dialog/Confirm'
import HTML from '@/shared/Input/Html'

export default {
    data() {
        return {
            edit: false,
            changes: ""
        }
    },
    created() {
        this.getBuilder();    
    },
    components: {
        'l-avatar': Avatar,
        'l-timestamp': Timestamp,
        'l-confirm': Confirm,
        'l-html': HTML
    },
    props: {
        comment: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        },
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {}
        },
        builder() {
            let builders = this.$store.getters['auth/builders'];
            let builder = Object.keys(builders).map(key => builders[key])
            .filter( item => item !== null && item.id === this.comment.userId)[0];
            return (builder) ? builder : {};
        },
        auth() {
            return this.comment.userId === this.user.id;
        },
        to() {
            let username = (this.builder.username === undefined) ? "notfound" : this.builder.username;
            return { name: "builder-display", params: { display: username } };
        }
    },
    methods: {
        confirm(func) {
            func();
        },
        getBuilder() {
            if (this.comment.userId !== undefined) return;
            this.$store.dispatch('auth/getBuilderById', this.comment.userId);
        },
        modify() {
            this.changes = this.comment.text;
            this.edit = true;
        },
        remove() {
            console.log('remove');
            this.$store.dispatch('comments/removeComment', this.comment);
        },
        cancel() {
            this.edit = false;
            this.comment.text = this.changes;
        },
        save() {
            this.edit = false;
            this.$store.dispatch('comments/updateComment', this.comment );
        }
    }
}
</script>