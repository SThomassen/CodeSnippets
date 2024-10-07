<template>
<v-container>
    <l-confirm ref='dialog' @confirm='confirm'></l-confirm>
    
    <v-row no-gutters>
        <v-col>
            <v-list>
                <v-list-item>
                    <v-text-field 
                        outlined
                        :label='$t("title")'
                        v-model='project.projectName'>
                    </v-text-field>
                </v-list-item>

                <v-list-item>
                    <vue-editor id="editor" style='width:100%;' v-model="project.description"> </vue-editor>
                </v-list-item>

                <v-list-item>
                    <l-tags v-model='project.tags' :items='tags'></l-tags>
                </v-list-item>

                <v-list>
                    <v-subheader>{{$t('general')}}</v-subheader>
                    <v-list-item>
                        <v-list-item-action>
                            <v-checkbox v-model="project.published">
                            </v-checkbox>
                        </v-list-item-action>
                        <v-list-item-content>
                            <v-list-item-title>{{$t('publish')}}</v-list-item-title>
                            <v-list-item-subtitle>{{$t('publish_project_hint')}}. </v-list-item-subtitle>
                        </v-list-item-content>
                    </v-list-item>
                    <v-list-item>
                        <v-list-item-action>
                            <v-checkbox v-model="project.allowComments"></v-checkbox>
                        </v-list-item-action>
                        <v-list-item-content>
                            <v-list-item-title>{{$t('allow_comments')}}</v-list-item-title>
                            <v-list-item-subtitle>{{$t('allow_comments_hint')}}.</v-list-item-subtitle>
                        </v-list-item-content>
                    </v-list-item>
                    <v-list-item>
                        <v-list-item-action>
                            <v-checkbox v-model="project.allowAccess"></v-checkbox>
                        </v-list-item-action>
                        <v-list-item-content>
                            <v-list-item-title>{{$t('allow_access')}}</v-list-item-title>
                            <v-list-item-subtitle>{{$t('allow_access_hint')}}.</v-list-item-subtitle>
                        </v-list-item-content>
                    </v-list-item>
                    <v-list-item>
                        <v-list-item-action>
                            <v-checkbox v-model="project.notifications"></v-checkbox>
                        </v-list-item-action>
                        <v-list-item-content>
                            <v-list-item-title>{{$t('notifications')}}</v-list-item-title>
                            <v-list-item-subtitle>{{$t('notifications_hint')}}.</v-list-item-subtitle>
                        </v-list-item-content>
                    </v-list-item>

                    <template v-if='hasPermission(["showcase_project"])'>
                        <v-list-item>
                            <v-list-item-action>
                                <v-checkbox v-model="project.showcase"></v-checkbox>
                            </v-list-item-action>
                            <v-list-item-content>
                                <v-list-item-title>{{$t('showcase')}}</v-list-item-title>
                                <v-list-item-subtitle>{{$t('showcase_hint')}}.</v-list-item-subtitle>
                            </v-list-item-content>
                        </v-list-item>
                    </template>


                    <v-divider class='mt-6'></v-divider>

                    <v-list-item-action>
                        <v-row>
                            <v-col v-if='hasPermission(["export_project"])'>
                                <v-btn 
                                    small 
                                    outlined 
                                    @click='download'>
                                    <v-icon left>mdi-download</v-icon>
                                    {{$t('export')}}
                                </v-btn>
                            </v-col>
                            <v-col>
                                <v-btn 
                                    small
                                    outlined 
                                    color='error' 
                                    @click='$refs.dialog.confirm({ 
                                        title: $t("messages.remove_project.title", { name: project.projectName}), 
                                        message: $t("messages.remove_project.message", { name: project.projectName}), 
                                        callback: remove
                                    })'>
                                    <v-icon left>mdi-delete</v-icon>
                                    {{$t('delete_project')}}
                                </v-btn>
                            </v-col>
                        </v-row>
                    </v-list-item-action>
                </v-list>
            </v-list>
        </v-col>
    </v-row>
</v-container>
</template>

<script>
import { VueEditor, Quill } from "vue2-editor";
import Confirm from '@/shared/Dialog/Confirm'
import Tags from '@/shared/Input/Tags'

export default {
    data() {
        return {
            tags: [
                { header: 'Select an option or create one' },
                { text: 'Stage' },
                { text: 'Event' },
                { text: 'Camping' },
                { text: 'Visual show' },
            ]
        }
    },
    components: {
        'l-tags': Tags,
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
    methods: {
        confirm(func) {
            func();
        },
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission'](perms);
        },
        async download() {
            let project = await this.$store.dispatch('projects/exportProject', { id: this.project.id, lastSave: this.project.lastSave });
            let json = JSON.stringify(project);
             var pom = document.createElement('a');
            var blob = new Blob([json],{type: 'application/json;charset=utf-8;'});
            var url = URL.createObjectURL(blob);
            pom.href = url;
            
            pom.setAttribute('download', "project_"+this.project.projectName);
            pom.click();
        },
        async remove() {
            await this.$store.dispatch('projects/invokeProject', {
                projectId: this.project.id,
                creatorId: this.project.creatorId
            });
            this.$router.go(-1);
        }
    }
}
</script>