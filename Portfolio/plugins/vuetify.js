import Vue from 'vue';
import Vuetify from 'vuetify';

import theme from '@/config/themes';
import 'vuetify/dist/vuetify.min.css';

export default new Vuetify({
    theme: { dark: true }
});
Vue.use(Vuetify);
