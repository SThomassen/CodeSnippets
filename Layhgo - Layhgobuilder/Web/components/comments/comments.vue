<template>
    <v-container fluid>
        <v-form ref='commentForm' @submit.prevent='submit'>
            <template v-if='user'>
                <v-row align="stretch">
                    <v-col cols='auto'>
                        <v-avatar>
                            <l-avatar :builder='user'></l-avatar>
                        </v-avatar>
                    </v-col>
                    <v-col class="mt-1">
                        <v-textarea 
                            dense
                            outlined
                            auto-grow
                            :rows='1'
                            :row-height="24"
                            :rules='rules.messageRules'
                            :placeholder="$t('place_comment')"
                            v-model='text'>
                            <template v-slot:message='item'>
                                {{ $t(item.message) }}
                            </template>
                        </v-textarea>
                    </v-col>
                    
                    <v-col class="mt-1" cols='auto'>
                        <v-btn 
                            outlined 
                            color='info' 
                            type='submit'
                            :loading='loading'>
                            {{$t('submit')}}
                        </v-btn>
                    </v-col>
                </v-row>
            </template>

            <template v-else>
                <v-row no-gutters>
                    <v-col cols='4'>
                        <v-text-field 
                            dense
                            outlined
                            class='mr-4'
                            :rules='rules.nameRules'
                            :placeholder="$t('display_name')"
                            v-model='displayName' >
                            <template v-slot:message='item'>
                                {{ $t(item.message) }}
                            </template>
                        </v-text-field>
                    </v-col>
                    <v-col cols='4'>
                        <v-text-field 
                            dense
                            outlined
                            class='mr-4'
                            :rules='rules.emailRules'
                            :placeholder="$t('email')"
                            v-model='email' >
                            <template v-slot:message='item'>
                                {{ $t(item.message) }}
                            </template>
                        </v-text-field>
                    </v-col>
                    <v-spacer></v-spacer>
                    <v-col cols='auto'>
                        
                        <v-btn 
                            outlined 
                            color='info' 
                            type='submit'
                            :loading='loading'>
                            {{$t('submit')}}
                        </v-btn>
                    </v-col>
                </v-row>
                <v-row no-gutters>
                    <v-textarea 
                        dense
                        outlined
                        auto-grow
                        :rows='1'
                        :row-height="24"
                        :rules='rules.messageRules'
                        :placeholder="$t('place_comment')"
                        v-model='text'>
                        <template v-slot:message='item'>
                            {{ $t(item.message) }}
                        </template>
                    </v-textarea>
                </v-row>
            </template>
        </v-form>

        <v-list v-if='showComments && comments.length > 0'>
            <v-list-item v-for='comment in comments.slice(start, end)' :key='comment.id'>
                <l-comment :comment='comment'></l-comment>
                <v-divider></v-divider>
            </v-list-item>
            <v-row class='mt-1' align="center" justify="end">
                <v-spacer></v-spacer>
                <v-col cols='auto'>
                    {{this.start}} - {{this.end}} {{ $t('of')}} {{this.total}}
                </v-col>
                <v-col cols='auto'>
                    <v-pagination
                        v-model="pagination.page"
                        :length="pages"
                        :total-visible="0"
                        @next="getComments">
                    </v-pagination>
                </v-col>
            </v-row>
        </v-list>
    </v-container>
</template>

<script>
import Comment from '@/components/comments/comment'
import Avatar from '@/shared/Cards/Avatar'

export default {
    data() {
        return {
            text: "",
            displayName: "",
            email: "",
            loading: false,
            pagination: {
                page: 1,
                itemsPerPage: 10
            },
            rules: {
                nameRules: [
                    v => !!v || 'rules.name_required',
                    v => (v && v.length <= 20) || 'rules.name_length'
                ],
                emailRules: [
                    v => !!v || 'rules.email_required',
                    v => /.+@.+/.test(v) || 'rules.email_valid'
                ],
                messageRules: [
                    v => !!v || 'rules.message_required',
                    v => (v && v.length >= 4) || 'rules.message_length'
                ]
            },
        }
    },
    created() {
        this.getComments();
    },
    components: {
        'l-comment': Comment,
        'l-avatar': Avatar
    },
    props: {
        type: {
            type: String,
            required: true,
            default: "projects"
        },
        target: {
            type: Object,
            required: true,
            default: function() {
                return {}
            }
        },
        requireUser: {
            type: Boolean,
            required: false,
            default: true
        },
        showComments: {
            type: Boolean,
            required: false,
            default: true
        }
    },
    computed: {
        user() {
            return this.$store.getters['auth/user'];
        },
        comments() {
            let comments = this.$store.getters['comments/comments'];
            return Object.values(comments).filter( item => { return item.type === this.type && item.source === this.target.id && !item.invoked }).sort(function(a,b) { return b.time - a.time; });
        },
        stats() {
            return (this.target['--stats--']) ? this.target['--stats--'] : {};
        },
        pages() {
            let size = Math.ceil(this.total / this.pagination.itemsPerPage);    
            return (size <= 0 || isNaN(size)) ? 1 : size;
        },
        total() {
            return (this.stats && this.stats.comments) ? this.stats.comments : 1; // needs to be -1 to exclude current user
        },
        start() {
            return (this.pagination.page - 1) * this.pagination.itemsPerPage;
        },
        end() {
            return this.start + this.pagination.itemsPerPage;
        },
    },
    methods: {
        async submit() {
            if (!this.$refs.commentForm.validate()) {
                return;
            }

            let comment = {
                invoked: false,
                text: this.text,
                type: this.type,
                source: this.target.id,
                userId: (this.user && this.user.id) ? this.user.id : null,
                displayName: (this.user && this.user.id) ? this.user.displayName : this.displayName
            }
            let id = await this.$store.dispatch('comments/setComment', comment);
            this.text = "";
            this.$emit('onSubmit', id);
        },
        getComments() {
            let src = this.target.id ? this.target.id : this.$route.query.id;
            this.$store.dispatch('comments/getComments', {  limit: this.pagination.itemsPerPage + 1, source: src });
        }
    },
}
</script>