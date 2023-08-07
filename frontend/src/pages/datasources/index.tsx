import { caches, dbs, queues } from "@/mock/datasources";
import { Typography } from "@/components/ui/typography";

import DatasourceCard from "./components/datasource-card";
import OuterCard from "./components/outer-card";

const Datasources = () => {
  return (
    <div className="container mt-8 flex flex-col gap-8">
      <div className="flex justify-center">
        <Typography variant="h1">Datasources</Typography>
      </div>

      <OuterCard title="Caches" description="Here you can preview and edit your caches.">
        {caches.map((cache) => (
          <DatasourceCard key={cache.id} datasource={cache} />
        ))}
      </OuterCard>

      <OuterCard title="Databases" description="Here you can preview and edit your databases.">
        {dbs.map((database) => (
          <DatasourceCard key={database.id} datasource={database} />
        ))}
      </OuterCard>

      <OuterCard title="Queues" description="Here you can preview and edit your queues.">
        {queues.map((queue) => (
          <DatasourceCard key={queue.id} datasource={queue} />
        ))}
      </OuterCard>
    </div>
  );
};

export default Datasources;
