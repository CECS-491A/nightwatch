import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '../components/HelloWorld'
import Login from '@/views/Login'
import Dashboard from '../views/Dashboard'
import AdminDashboard from '../views/AdminDashboard'
import MapView from '../views/MapView'
import PointDetails from '../views/PointDetails'

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
      path: '/pointdetails',
      name: 'PointDetails',
      component: PointDetails
    },
    {
      path: '/pointeditor',
      name: 'PointEditor',
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
