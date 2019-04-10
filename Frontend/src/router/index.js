import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '../components/HelloWorld'
import Login from '@/views/Login'
import Dashboard from '../views/Dashboard'
import AdminDashboard from '../views/AdminDashboard'
import MapView from '../views/MapView'
import Account from '@/views/Account'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'HelloWorld',
      component: HelloWorld
    },
    {
      path: '/dashboard',
      name: 'Dashboard',
      component: Dashboard
    },
    {
      path: '/admindashboard',
      name: 'AdminDashboard',
      component: AdminDashboard
    },
    {
      path: '/mapview',
      name: 'MapView',
      component: MapView
    },
    {
      path: '/account',
      name: 'Account',
      component: Account
    },
    {
      path: '*',
      component: HelloWorld
    },
    {
      path: '/login',
      name: 'Login',
      component: Login
    }
  ]
})
