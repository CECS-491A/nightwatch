<template>
  <div>
     <div v-if="loading">
      <Loading :dialog="loading" :text="loadingText"/>
    </div>
    <div v-if="!validSession">
      <PopupDialog :dialog="!validSession" :text="popupMessage" redirect=true :redirectUrl="redirectUrl"/>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import Loading from '@/components/dialogs/Loading'
import PopupDialog from '@/components/dialogs/PopupDialog'
import { GetUser } from '@/services/userManagementServices'
import { store } from '@/services/accountServices.js'
import { httpResponseCodes } from '@/services/services.const.js'

export default {
  name: 'Login',
  components: {
    Loading,
    PopupDialog
  },
  data: () => ({
    token: '',
    loading: false,
    loadingText: '',
    validSession: true,
    popupMessage: '',
    redirectUrl: 'https://kfc-sso.com'
  }),
  mounted() {
    this.token = this.$route.query.token;
    localStorage.setItem('token', this.token);
    this.CheckUser(this.token);
  },
  methods: {
    CheckUser(token) {
      this.loading = true;
      this.loadingText = 'Logging In...';
      GetUser()
        .then( response => {
          switch(response.status){
            case httpResponseCodes.OK: // status OK
              var user = response.data;
              store.state.isLogin = true;
              store.getEmail();
              localStorage.setItem('token', this.token);
              this.loading = false;
              this.loadingText = '';
              if (user.isAdmin){
                this.$router.push('/admindashboard');
              }
              else{
                this.$router.push('/mapview');
              }
            default:
          }
        })
        .catch( err => {
          this.loading = false;
          this.loadingText = '';
          if (err.response){
            switch(err.response.status){
              case httpResponseCodes.NotFound: // status Not Found
              case httpResponseCodes.Unauthorized: // status Unauthorized
                this.loading = false;
                this.popupMessage = 'The session has expired...';
                this.validSession = false;
                break;
              default:
            }
          } 
          else{
            this.loading = false;
            this.popupMessage = 'This application has encounted a problem...';
            this.validSession = false;
          }
        })
    }
  }
}
</script>