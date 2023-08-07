import { AlertTriangle, CheckIcon, X } from "lucide-react";

import { FrontBack } from "@/mock/overview";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

interface IConfigOverviewCardProps {
  title: string;
  description: string;
  upNum: number;
  totalNum: number;
  viewAllURL: string;
  frontBackList: FrontBack[];
}

const ConfigOverviewCard = ({
  title,
  description,
  upNum,
  totalNum,
  viewAllURL,
  frontBackList,
}: IConfigOverviewCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex justify-between">
          {title}: {upNum}/{totalNum} up
          <Button asChild variant="outline" className="-mt-1">
            <a href={viewAllURL}>View all</a>
          </Button>
        </CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent className="grid gap-6">
        {frontBackList.slice(0, 3).map((frontBack) => (
          <div key={frontBack.name} className="flex items-center justify-between space-x-4">
            <div className="flex items-center space-x-4">
              {frontBack.status === "up" && <CheckIcon color="#10B981" />}
              {frontBack.status === "down" && <X color="#EF4444" />}
              {frontBack.status === "warn" && <AlertTriangle color="#FBBF24" />}
              <div>
                <p className="text-sm font-medium leading-none">{frontBack.name}</p>
                <p className="text-sm text-muted-foreground">{frontBack.path}</p>
              </div>
            </div>
            <Button variant="ghost">View spec</Button>
          </div>
        ))}
      </CardContent>
    </Card>
  );
};

export default ConfigOverviewCard;
