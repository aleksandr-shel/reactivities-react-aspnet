import { Redirect, Route, RouteChildrenProps, RouteProps } from "react-router-dom";
import { useStore } from "../stores/store";

interface Props extends RouteProps{
    component: React.ComponentType<RouteChildrenProps<any>> | React.ComponentType<any>;
}

export default function PrivateRoute({component : Component, ...rest}:Props){
    const {userStore: {isLoggedIn}} = useStore();

    return(
        <Route 
            {...rest}
            render={(props)=> isLoggedIn ? <Component {...props}/> : <Redirect to='/'/>}
        />
    )
}