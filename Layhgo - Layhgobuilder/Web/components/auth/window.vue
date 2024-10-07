<template>
    <v-dialog 
        @keydown.esc="cancel"
        v-model="dialog"
        max-width="600px"
        transition="dialog-bottom-transition">

        <v-card>
            <v-container fluid>

                <v-row no-gutters justify='end'>
                    <v-btn icon small @click='cancel'>
                        <v-icon>mdi-close</v-icon>
                    </v-btn>
                </v-row>

                <v-row no-gutters justify='center' align='center' class='text-center'>
                    <v-col cols='12'>
                        <span class='display-1 font-weight-bold'>{{ $t(title) }}</span>
                    </v-col>

                    <v-col cols='12'>
                        <span class='caption font-weight-light'>You are the builder</span>
                    </v-col>
                </v-row>
                
                <v-window v-model="tab">
                    <v-window-item :value='0'>
                        <l-login @signup='open(1)' @forgot='open(2)' @fieldlab='fieldlab' @enterprise='enterprise' @cancel='cancel()'></l-login>
                    </v-window-item> 
                    <v-window-item :value='1'>
                        <l-signup @login='open(0)' @cancel='cancel()'></l-signup>
                    </v-window-item>
                    <v-window-item :value='2'>
                        <l-forgot @cancel='open(0)'></l-forgot>
                    </v-window-item>
                    <v-window-item :value='3'>
                        <l-license @request='open(4)' @cancel='cancel()'></l-license>
                    </v-window-item>
                    <v-window-item :value='4'>
                        <l-request :product='product' @cancel='cancel()'></l-request>
                    </v-window-item>
                </v-window>

            </v-container>
        </v-card>
    </v-dialog>
</template>

<script>
import Login from './Login'
import Signup from './Signup'
import Forgot from './Forgot'
import License from './license'
import Request from './request'

export default {
    components: {
        'l-login': Login,
        'l-signup': Signup,
        'l-forgot': Forgot,
        'l-license': License,
        'l-request': Request
    },
    data() {
        return {
            dialog: false,
            tabs: [
                { title: 'log_in' },
                { title: 'sign_up' },
                { title: 'forgot_password' },
                { title: 'license' },
                { title: 'products.request_license' }
            ],
            tab: 0,
            product: 'Fieldlab'
        }
    },
    computed: {
        title() {
            return this.tabs[this.tab].title;
        }
    },
    methods: {
        cancel() {
            this.dialog = false;
            this.$bus.$emit('onCloseForm');
        },
        open(index) {
            this.tab = index;
            this.dialog = true;
        },
        setProduct(product) {
            this.product = product;
        },
        fieldlab() {
            this.tab = 3;
            this.product = "fieldlab";
        },
        enterprise() {
            this.tab = 3;
            this.product = "enterprise";
        }
    }
}
</script>