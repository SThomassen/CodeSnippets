<template>
    <v-container fluid class='mt-4'>
        <v-card>
            <v-row>
                <v-col>

                </v-col>
                <v-col>

                </v-col>
            </v-row>

            <v-row justify="center" class='pa-2'>
            
                <v-col cols='12'>
                    <h3>About me</h3>
                </v-col>
                <v-col>
                    <v-textarea outlined></v-textarea>
                </v-col>
            
        </v-row>
        </v-card>
    </v-container>
</template>

<script>
import info from '@/config/info'
import Textfield from '@/shared/Input/Textfield'
import Select from '@/shared/Input/Select'
import BirthdayPicker from '@/shared/Input/BirthdayPicker'

export default {
    components: {
        'l-textfield': Textfield,
        'l-select': Select,
        'l-birthday-picker': BirthdayPicker
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
            edit: false,
            loading: false
        }
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user']
            return (user) ? user : {};
        },
        auth() {
            return this.builder.id === this.user.id;
        },
        countries() {
            return info.countries;
        },
        changes() {
            return JSON.stringify(this.user) !== JSON.stringify(this.builder);
        }
    },
    methods: {
        async save() {
            this.loading = true;
            console.log(this.builder);
            await this.$store.dispatch('auth/updateUser', this.builder);
            this.loading = false;
            this.edit = false;
        }
    }
}
</script>