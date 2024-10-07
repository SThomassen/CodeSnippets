<template>
    <span>
        <l-confirm ref='dialog' @confirm='confirm'></l-confirm>

        <v-list-item>
            <v-list-item-avatar>
                <l-avatar :builder='builder'></l-avatar>
            </v-list-item-avatar>

            <v-list-item-title>
                {{builder.displayName}}
            </v-list-item-title>

            <v-list-item-action>
                <v-select 
                    flat
                    solo
                    dense
                    hide-details
                    :items='items'
                    :value='input'
                    @change='set'>
                </v-select>
            </v-list-item-action>
            <v-list-item-action>
                <v-btn 
                    icon 
                    small 
                    @click='$refs.dialog.confirm({ 
                        title: $t("messages.remove_user.title", { name: builder.displayName }), 
                        message: $t("message.remove_user.message", { name: builder.displayName }), 
                        callback: remove
                    })'>
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-list-item-action>
        </v-list-item> 
    </span>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Confirm from '@/shared/Dialog/Confirm'
import { access } from '@/config/security'

export default {
    mounted() {
        this.getBuilder();
    },
    props: {
        id: {
            type: String,
            required: true,
            default: ""
        },
        value: {
            type: String,
            required: true,
            default: 'dUUuBLsma7qxvjk5Lypd'
        }
    },
    components: {
        'l-avatar': Avatar,
        'l-confirm': Confirm
    },
    computed: {
        items() {
            return Object.values(access);
        },
        input() {
            return this.value;
        },
        builder() {
            let builders = this.$store.getters['auth/builders']
            let builder = Object.values(builders).find(builder => { return builder.id === this.id });
            return (builder) ? builder : {};
        }
    },
    methods: {
        confirm(func) {
            func();
        },
        set(value) {
            this.$emit('input', value);
        },
        remove() {
            this.$emit('remove', this.id);
        },
        getBuilder() {
            if (this.builder.id) return;
            this.$store.dispatch('auth/getBuilderById', this.id);
        }
    }
}
</script>