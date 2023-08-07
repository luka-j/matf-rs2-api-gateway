import { apis, backends, caches, databases, frontends, queues, rps } from "@/mock/overview";
import { Typography } from "@/components/ui/typography";

import ConfigOverviewCard from "./components/config-overview-card";
import DatasourceCard from "./components/datasource-card";
import OuterCard from "./components/outer-card";
import SystemOverviewCard from "./components/system-overview-card";

const Dashboard = () => {
  return (
    <main className="mx-auto mt-8 max-w-7xl space-y-8 pb-8">
      <Typography variant="h1" className="text-center">
        API Gateway Dashboard
      </Typography>

      <OuterCard title="Config Overview" description="Here you can preview and edit your configs.">
        <ConfigOverviewCard
          title="Frontends"
          description="Here you can preview and edit your frontends."
          upNum={5}
          totalNum={7}
          viewAllURL="/dashboard/frontends"
          frontBackList={frontends}
        />

        <ConfigOverviewCard
          title="Backends"
          description="Here you can preview and edit your backends."
          upNum={3}
          totalNum={7}
          viewAllURL="/dashboard/backends"
          frontBackList={backends}
        />
      </OuterCard>

      <OuterCard
        title="Datasources"
        description="Here you can preview and edit your datasources."
        viewAllURL="/dashboard/datasources"
      >
        <DatasourceCard
          title="Caches"
          upNum={5}
          totalNum={7}
          description="Here you can preview and edit your caches."
          viewAllURL="/dashboard/datasources/caches"
          datasourceList={caches}
        />

        <DatasourceCard
          title="Databases"
          upNum={3}
          totalNum={7}
          description="Here you can preview and edit your databases."
          viewAllURL="/dashboard/datasources/databases"
          datasourceList={databases}
        />

        <DatasourceCard
          title="Queues"
          upNum={1}
          totalNum={7}
          description="Here you can preview and edit your queues."
          viewAllURL="/dashboard/datasources/queues"
          datasourceList={queues}
        />
      </OuterCard>

      <OuterCard title="System Overview" description="Here you can preview and edit your systems.">
        <SystemOverviewCard
          title="APIs"
          numInstances={5}
          viewAllURL="/dashboard/systems/apis"
          description="Here you can preview and edit your APIs."
          systemOverviewList={apis}
          seeMetricsURL="/dashboard/metrics/apis"
        />

        <SystemOverviewCard
          title="RPs"
          numInstances={3}
          viewAllURL="/dashboard/systems/rps"
          description="Here you can preview and edit your RPs."
          systemOverviewList={rps}
          seeMetricsURL="/dashboard/metrics/rps"
        />
      </OuterCard>
    </main>
  );
};

export default Dashboard;
