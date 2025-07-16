import OpenIdProviderService from "./service"
import { ModuleProvider, Modules, ContainerRegistrationKeys } from "@medusajs/framework/utils"

export default ModuleProvider(Modules.AUTH, {
    services: [OpenIdProviderService],
    dependencies: [ContainerRegistrationKeys.CUSTOMER_SERVICE],
})